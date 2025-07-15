namespace Nabunassar.Entities
{
    internal interface IEntity
    {
        Guid ObjectId { get; set; }

        string FormulaName { get; set; }
    }

    public class DescribeEntity : IEntity
    {
        public DescribeEntity()
        {
            ObjectId = Guid.NewGuid();
        }

        public Guid ObjectId { get;set; }

        public string FormulaName { get; set; }
    }
}
