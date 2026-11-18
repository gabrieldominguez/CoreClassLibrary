namespace CoreClassLibrary.Collections;

/// <summary>
/// Implementa una pila de undo/redo del tipo especificado para operaciones.
/// </summary>
/// <Remarks>
/// A diferencia de UndoRedoOperationStack, una UndoRedoOperationStack no soporta el concepto 
/// de item actual (Current)
/// </Remarks>
public class UndoRedoOperationStack<T>
    {
    private readonly BoundedStack<T?> undoStack;
    private readonly BoundedStack<T?> redoStack;
    
    public UndoRedoOperationStack(int capacity)
        {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacity);

        this.undoStack = new BoundedStack<T?>(capacity);
        this.redoStack = new BoundedStack<T?>(capacity);
        }

    public class UndoRedoEventArgs(T? item) : EventArgs
        {
        public readonly T? Item = item;

        public UndoRedoEventArgs() : this(default) {}
        }

    public delegate void UndoRedoEventHandler(object sender, UndoRedoEventArgs e);

    public event UndoRedoEventHandler? Did;

    protected virtual void OnDid(UndoRedoEventArgs e) => this.Did?.Invoke(this, e);

    public event UndoRedoEventHandler? Undid;

    protected virtual void OnUndid(UndoRedoEventArgs e) => this.Undid?.Invoke(this, e);

    public event UndoRedoEventHandler? Redid;

    protected virtual void OnRedid(UndoRedoEventArgs e) => this.Redid?.Invoke(this, e);

    public event EventHandler? Cleared;

    protected virtual void OnCleared(EventArgs e) => this.Cleared?.Invoke(this, e);

    public bool CanDo => this.undoStack.Capacity > 0;

    public void Do(T item)
        {
        if(this.CanDo)
            {
            this.undoStack.Push(item);
            this.redoStack.Clear();
            this.OnDid(new UndoRedoEventArgs(item));
            }
        }

    public bool CanUndo => this.undoStack.Count > 0;

    public void Undo(object sender, EventArgs e) => this.Undo();

    public T? Undo()
        {
        if(this.CanUndo)
            {
            T? item = this.undoStack.Pop();
            this.redoStack.Push(item);
            this.OnUndid(new UndoRedoEventArgs(item));
            return item;
            }

        return default;
        }

    public bool CanRedo => this.redoStack.Count > 0;

    public void Redo(object sender, EventArgs e) => this.Redo();

    public T? Redo()
        {
        if(this.CanRedo)
            {
            T? item = this.redoStack.Pop();
            this.undoStack.Push(item);
            this.OnRedid(new UndoRedoEventArgs(item));
            return item;
            }

        return default;
        }


    public void Clear()
        {
        this.undoStack.Clear();
        this.redoStack.Clear();

        this.OnCleared(EventArgs.Empty);
        }

    public void SetCapacity(int capacity)
        {
        this.undoStack.SetCapacity(capacity);
        this.redoStack.SetCapacity(capacity);
        }
    }
