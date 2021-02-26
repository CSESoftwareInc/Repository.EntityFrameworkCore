namespace CSESoftware.Repository.EntityFrameworkCore.TestProject.Setup
{
    public class PersonPizza
    {
        public int PersonId { get; set; }
        public Person Person { get; set; }
        public int PizzaId { get; set; }
        public Pizza Pizza { get; set; }
    }
}
