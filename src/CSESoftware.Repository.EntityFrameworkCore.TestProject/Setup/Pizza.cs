using System.Collections.Generic;

namespace CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup
{
    public class Pizza
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public double Cost { get; set; }

        public int CrustId { get; set; }
        public Crust Crust { get; set; }

        public int ToppingId { get; set; }
        public Topping Topping { get; set; }

        public ICollection<PersonPizza> PersonPizzas { get; set; }
    }
}
