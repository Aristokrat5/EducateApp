using EducateApp.Models;
using EducateApp.Models.Data;
using EducateApp.ViewModels.Disciplines;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using System;
using System.IO;

namespace EducateApp.Controllers
{
    [Authorize(Roles = "admin, registeredUser")]
    public class DisciplinesController : Controller
    {
        private readonly AppCtx _context;
        private readonly UserManager<User> _userManager;

        public DisciplinesController(
            AppCtx context,
            UserManager<User> user)
        {
            _context = context;
            _userManager = user;
        }

        // GET: Disciplines
        public async Task<IActionResult> Index()
        {
            // находим информацию о пользователе, который вошел в систему по его имени
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            var appCtx = _context.Disciplines
 .Include(d => d.User)
 .Where(w => w.IdUser == user.Id)
 .OrderBy(o => o.Name);
            return View(await appCtx.ToListAsync());
        }


        // GET: Disciplines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Disciplines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDisciplinesViewModel model)
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.Disciplines.Where(f => f.IdUser == user.Id &&
                    f.Name == model.Name).FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введенный вид дисциплины уже существует");
            }

            if (ModelState.IsValid)
            {
                Disciplines disciplines = new()
                {
                    IndexProfModule = model.IndexProfModule,
                    ProfModule = model.ProfModule,
                    Index = model.Index,
                    Name = model.Name,
                    ShortName = model.ShortName,
                    IdUser = user.Id
                };

                _context.Add(disciplines);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Disciplines/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disciplines = await _context.Disciplines.FindAsync(id);
            if (disciplines == null)
            {
                return NotFound();
            }

            EditDisciplinesViewModel model = new()
            {
                Id = disciplines.Id,
                IndexProfModule = disciplines.IndexProfModule,
                ProfModule = disciplines.ProfModule,
                Index = disciplines.Index,
                Name = disciplines.Name,
                ShortName = disciplines.ShortName,
                IdUser = disciplines.IdUser
            };


            return View(model);
        }

        // POST: Disciplines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, EditDisciplinesViewModel model)
        {
            Disciplines disciplines = await _context.Disciplines.FindAsync(id);

            if (id != disciplines.Id)
            {
                return NotFound();
            }

            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (_context.Disciplines
                .Where(f => f.IdUser == user.Id &&
                    f.Name == model.Name).FirstOrDefault() != null)
            {
                ModelState.AddModelError("", "Введенный вид дисциплины уже существует");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(disciplines);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisciplinesExists(disciplines.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Disciplines/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disciplines = await _context.Disciplines
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (disciplines == null)
            {
                return NotFound();
            }

            return View(disciplines);
        }

        // POST: Disciplines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            var disciplines = await _context.Disciplines.FindAsync(id);
            _context.Disciplines.Remove(disciplines);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Disciplines/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var disciplines = await _context.Disciplines
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (disciplines == null)
            {
                return NotFound();
            }

            return PartialView(disciplines);
        }



        public async Task<FileResult> DownloadPattern()
        {
            IdentityUser user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            // выбираем из базы данных все специальности текущего пользователя
            var appCtx = _context.Disciplines
                .Include(d => d.User)
                 .Where(w => w.IdUser == user.Id)
                 .OrderBy(o => o.Name);

            int i = 1;      // счетчик

            IXLRange rngBorder;     // объект для работы с диапазонами в Excel (выделение групп ячеек)

            // создание книги Excel
            using (XLWorkbook workbook = new(XLEventTracking.Disabled))
            {
                // для каждой специальности 
                foreach (Disciplines disciplines in appCtx)
                {
                    // добавить лист в книгу Excel
                    // с названием 3 символа формы обучения и кода специальности
                    IXLWorksheet worksheet = workbook.Worksheets
                        .Add($"{disciplines.Id}");

                    // в первой строке текущего листа указываем: 
                    // в ячейку A1 значение "Форма обучения"
                    worksheet.Cell("A" + i).Value = "Индекс проф. модуля";
                    // в ячейку B1 значение - название формы обучения текущей специальности
                    worksheet.Cell("B" + i).Value = disciplines.IndexProfModule;
                    // увеличение счетчика на единицу
                    i++;

                    // во второй строке
                    worksheet.Cell("A" + i).Value = "Проф. Модуль";
                    worksheet.Cell("B" + i).Value = $"'{disciplines.ProfModule}";
                    i++;
                    worksheet.Cell("A" + i).Value = "Индекс";
                    worksheet.Cell("B" + i).Value = disciplines.Index;

                    i++;
                    worksheet.Cell("A" + i).Value = "Название";
                    worksheet.Cell("B" + i).Value = disciplines.Name;

                    i++;
                    worksheet.Cell("A" + i).Value = "Сокращенное название";
                    worksheet.Cell("B" + i).Value = disciplines.ShortName;

                    // устанавливаем внешние границы для диапазона A4:F4
                    rngBorder = worksheet.Range("A1:B5");       // создание диапазона (выделения ячеек)
                    rngBorder.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;       // для диапазона задаем внешнюю границу

                    // на листе для столбцов задаем значение ширины по содержимому
                    worksheet.Columns().AdjustToContents();

                    // счетчик "обнуляем"
                    i = 1;
                }

                // создаем стрим
                using (MemoryStream stream = new())
                {
                    // помещаем в стрим созданную книгу
                    workbook.SaveAs(stream);
                    stream.Flush();

                    // возвращаем файл определенного типа
                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"Disciplines_{DateTime.UtcNow.ToShortDateString()}.xlsx"     //в названии файла указываем таблицу и текущую дату
                    };
                }
            }
        }



        private bool DisciplinesExists(short id)
        {
            return _context.Disciplines.Any(e => e.Id == id);
        }
    }
}