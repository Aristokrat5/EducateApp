using EducateApp.Models.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace EducateApp.ViewModels.Disciplines
{
    public class FilterDisciplineViewModel
    {
        public string SelectedIndexProfModule { get; private set; }    
        public string SelectedProfModule { get; private set; }    
        public string SelectedIndex { get; private set; }
        public string SelectedName { get; private set; }
        public string SelectedShortName { get; private set; }



        public FilterDisciplineViewModel(string indexProfModule, string profModule, string index, string name, string shortName)
        {
            SelectedIndexProfModule = indexProfModule;
            SelectedProfModule = profModule;
            SelectedIndex = index;
            SelectedName = name;
            SelectedShortName = shortName;

            /*// устанавливаем начальный элемент, который позволит выбрать всех
            formOfStudies.Insert(0, new FormOfStudy { FormOfEdu = "", Id = 0 });

            FormOfStudies = new SelectList(formOfStudies, "Id", "FormOfEdu", formOfEdu);
            FormOfEdu = formOfEdu;*/
        }
    }
}