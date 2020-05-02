using CSESoftware.Core.Entity;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup
{
    public class Crust : BaseEntity<int>
    {
        public string Name { get; set; }
        public double AdditionalCharge { get; set; }
    }
}