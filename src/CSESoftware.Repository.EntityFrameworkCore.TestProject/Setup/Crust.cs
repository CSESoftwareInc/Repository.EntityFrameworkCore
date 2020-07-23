using CSESoftware.Core.Entity;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup
{
    public class Crust : EntityWithId<int>
    {
        public string Name { get; set; }
        public double AdditionalCost { get; set; }
    }
}