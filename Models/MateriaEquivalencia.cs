namespace tutorias.Models
{
    public class MateriaEquivalencia
    {
        public int Id { get; set; }

        public int MateriaOrigenId { get; set; }
        public Materia? MateriaOrigen { get; set; }

        public int MateriaEquivalenteId { get; set; }
        public Materia? MateriaEquivalente { get; set; }

        public bool EsBidireccional { get; set; } = true;
    }
}
