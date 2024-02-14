namespace Homies.Models.Event
{
    public class EventInfoViewModel
    {
        public EventInfoViewModel(int id,string name, string start, string type, string organiser)
        {
            this.Id = id;
            this.Name = name;
            this.Start = start;
            this.Type = type;
            this.Organiser = organiser;
            
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Start {  get; set; }
        public string Type { get; set; }
        public string Organiser { get; set; }
    }
}
