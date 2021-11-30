using EducateApp.Models.Data;
using System.Collections.Generic;


namespace EducateApp.ViewModels.Disciplines
{
    public class IndexDisciplinesViewModel
    {
        public IEnumerable<Models.Data.Disciplines> Disciplines { get; set; }
        public PageViewModel PageViewModel { get; set; }
        public FilterDisciplineViewModel FilterDisciplinesViewModel { get; set; }
        public SortDisciplinesViewModel SortDisciplinesViewModel { get; set; }
    }
}