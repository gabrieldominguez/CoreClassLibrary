namespace CoreClassLibrary.Collections;

/// <summary>
/// Implementa una pila de undo/redo de datos del tipo especificado.
/// </summary>
public class UndoRedoDataStack<T>
    {
    private readonly BoundedStack<T?> undoStack;
    private readonly BoundedStack<T?> redoStack;

    public UndoRedoDataStack(int capacity)
        {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);

        this.undoStack = new BoundedStack<T?>(capacity);
        this.redoStack = new BoundedStack<T?>(capacity);
        }

    private T? current = default;
    public T? Current
        {
        get
            {
            return this.current;
            }

        private set
            {
            this.current = value;
            this.OnCurrentChanged(EventArgs.Empty);
            }
        }

    public event EventHandler? CurrentChanged;

    protected virtual void OnCurrentChanged(EventArgs e) => this.CurrentChanged?.Invoke(this, e);

    private bool isInitialized = false;

    public bool CanDo => !this.IsRedoing && !this.IsUndoing && this.undoStack.Capacity > 0;

    public bool IsDoing { get; private set; } = false;

    public virtual void Do(T value)
        {
        if(this.CanDo && (Object.Equals(this.Current, default(T)) || !this.Current!.Equals(value)))
            {
            if(this.isInitialized)
                {
                this.undoStack.Push(this.Current);
                this.redoStack.Clear();
                }

            this.IsDoing = true;
            this.Current = value;
            this.IsDoing = false;

            this.isInitialized = true;
            }
        }

    public bool CanUndo => this.undoStack.Count > 0;

    public bool IsUndoing { get; private set; } = false;

    public void Undo(object sender, EventArgs e) => this.Undo();

    public virtual T? Undo()
        {
        if(this.CanUndo)
            {
            this.redoStack.Push(this.Current);

            this.IsUndoing = true;
            this.Current = this.undoStack.Pop();
            this.IsUndoing = false;
            }

        return this.Current;
        }

    public bool CanRedo => this.redoStack.Count > 0;

    public bool IsRedoing { get; private set; } = false;

    public void Redo(object sender, EventArgs e) => this.Redo();

    public virtual T? Redo()
        {
        if(this.CanRedo)
            {
            this.undoStack.Push(this.Current);

            this.IsRedoing = true;
            this.Current = this.redoStack.Pop();
            this.IsRedoing = false;
            }

        return this.Current;
        }

    public void Clear()
        {
        this.undoStack.Clear();
        this.redoStack.Clear();

        this.isInitialized = false;
        this.Current = default;
        }

    public void SetCapacity(int capacity)
        {
        this.undoStack.SetCapacity(capacity);
        this.redoStack.SetCapacity(capacity);
        }
    }
