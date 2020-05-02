using CSESoftware.Core.Entity;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup
{
    public class Pizza : BaseEntity<int>
    {
        public string Name { get; set; }
        public double Cost { get; set; }


        public int CrustId { get; set; }
        public Crust Crust { get; set; }

        public int ToppingId { get; set; }
        public Topping Topping { get; set; }
    }
}