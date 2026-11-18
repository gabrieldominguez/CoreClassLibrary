using System.ComponentModel;

namespace CoreClassLibrary.Objects;

/// <summary>
/// Permite que los datos de un objeto sean transaccionales.
/// </summary>
/// <example>
///     public class Customer : EditableObject
///         {
///         public Customer(string name, int age)
///             {
///             this.actualData.Add("Name", name);
///             this.actualData.Add("Age", age);
///             }
///     
///         public string Name
///             {
///             get
///                 {
///                 return (string)this.actualData["Name"];
///                 }
///             set
///                 {
///                 this.actualData["Name"] = value;
///                 }
///             }
///     
///         public int Age
///             {
///             get
///                 {
///                 return (int)this.actualData["Age"];
///                 }
///             set
///                 {
///                 this.actualData["Age"] = value;
///                 }
///             }
///         }
/// </example>
public abstract class EditableObject : IEditableObject
	{
	protected Dictionary<string, object> actualData, backupData;

	private bool inTxn = false;

	public EditableObject()
		{
		this.backupData = this.actualData = [];
		}

	public void BeginEdit()
		{
		if (!this.inTxn)
			{
			this.backupData = this.actualData;
			this.inTxn = true;
			}
		}

	public void CancelEdit()
		{
		if (this.inTxn)
			{
			this.actualData = this.backupData;
			this.inTxn = false;
			}
		}

    public void EndEdit() => this.inTxn = false;
    }
