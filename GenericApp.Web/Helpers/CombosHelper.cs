using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GenericApp.Web.Data.Entities;
using GenericApp.Web.Data;
using System.Collections.Generic;
using System.Linq;

namespace GenericApp.Web.Helpers
{
    public class CombosHelper : ICombosHelper
    {
        private readonly DataContext _context;

        public CombosHelper(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetComboCategories()
        {
            List<SelectListItem> list = _context.Categories.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = $"{t.Id}"
            })
                .OrderBy(t => t.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione una categoría...]",
                Value = "0"
            });
            return list;
        }

        public IEnumerable<SelectListItem> GetComboCities(int departmentId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            DepartmentEntity department = _context.Departments
                .Include(d => d.Cities)
                .FirstOrDefault(d => d.Id == departmentId);
            if (department != null)
            {
                list = department.Cities.Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = $"{t.Id}"
                })
                    .OrderBy(t => t.Text)
                    .ToList();
            }

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione una ciudad...]",
                Value = "0"
            });
            return list;
        }

        public IEnumerable<SelectListItem> GetComboCountries()
        {
            List<SelectListItem> list = _context.Countries.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = $"{t.Id}"
            })
                .OrderBy(t => t.Text)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione un país...]",
                Value = "0"
            });
            return list;
        }

        public IEnumerable<SelectListItem> GetComboDepartments(int countryId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            CountryEntity country = _context.Countries
                .Include(c => c.Departments)
                .FirstOrDefault(c => c.Id == countryId);
            if (country != null)
            {
                list = country.Departments.Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = $"{t.Id}"
                })
                    .OrderBy(t => t.Text)
                    .ToList();
            }

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione una provincia...]",
                Value = "0"
            });
            return list;
        }

        public IEnumerable<SelectListItem> GetComboTeams(int countryId)
        {
            List<SelectListItem> list = new List<SelectListItem>();
            CountryEntity country = _context.Countries
                .Include(c => c.Teams)
                .FirstOrDefault(c => c.Id == countryId);
            if (country != null)
            {
                list = country.Teams.Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = $"{t.Id}"
                })
                    .OrderBy(t => t.Text)
                    .ToList();
            }

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione un equipo...]",
                Value = "0"
            });
            return list;
        }

        public IEnumerable<SelectListItem> GetComboUserTypes()
        {
            List<SelectListItem> list = new List<SelectListItem>();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione un Tipo de Usuario...]",
                Value = "0"
            });

            list.Insert(0, new SelectListItem
            {
                Text = "Admin",
                Value = "1"
            });

            list.Insert(0, new SelectListItem
            {
                Text = "User",
                Value = "2"
            });

            return list;
        }

        public IEnumerable<SelectListItem> GetComboStates()
        {
            List<SelectListItem> list = _context.States.Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = $"{t.Id}"
            })
                .OrderBy(t => t.Value)
                .ToList();

            list.Insert(0, new SelectListItem
            {
                Text = "[Seleccione un estado...]",
                Value = "0"
            });
            return list;
        }
    }
}