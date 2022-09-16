namespace PortFolio.Models
{

    [Table("Skills")]
    public class Skill
    {

        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Completion { get; set; }


        [DefaultValue(0)]
        public int Etat { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime SkillDate { get; set; }


    }
}
