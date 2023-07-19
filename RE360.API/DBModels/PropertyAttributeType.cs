namespace RE360.API.DBModels
{
    public class PropertyAttributeType
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<PropertyAttribute> PropertyAttribute { get; set; }

    }
}
