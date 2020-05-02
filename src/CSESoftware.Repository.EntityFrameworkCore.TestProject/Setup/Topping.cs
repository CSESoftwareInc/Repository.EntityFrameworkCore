using CSESoftware.Core.Entity;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup
{
    public class Topping : BaseEntity<int>
    {
        public string Name { get; set; }
        public double AdditionalCost { get; set; }
    }
}