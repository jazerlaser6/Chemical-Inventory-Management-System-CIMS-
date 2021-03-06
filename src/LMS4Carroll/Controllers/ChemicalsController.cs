using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LMS4Carroll.Data;
using LMS4Carroll.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LMS4Carroll.Controllers
{
    [Authorize(Roles = "Admin,ChemUser,BiologyUser,Student")]
    public class ChemicalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration configuration;

        public ChemicalsController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            this.configuration = config;
        }

        // GET: Chemicals
        public async Task<IActionResult> Index(string chemstring, string sortOrder)
        {
            ViewData["CurrentFilter"] = chemstring;
            sp_Logging("1-Info", "View", "Successfuly viewed Chemicals list", "Success");
            var chemicals = from m in _context.Chemical
                                select m;

            //Search Feature
            if (!String.IsNullOrEmpty(chemstring))
            {
                
                int forID;
                if (Int32.TryParse(chemstring, out forID))
                {
                    chemicals = chemicals.Where(s => s.ChemID.Equals(forID));
                }
                else
                {
                
                    chemicals = chemicals.Where(s => s.CAS.Contains(chemstring)
                                       || s.Formula.ToLower().Contains(chemstring)
                                       || s.FormulaName.ToLower().Contains(chemstring)
                                       || s.FormulaWeight.ToLower().Contains(chemstring)
                                       || s.CAT.ToLower().Contains(chemstring)
                                       || s.CAS.ToLower().Contains(chemstring)
                                       || s.Hazard.ToLower().Contains(chemstring)
                                       || s.State.ToLower().Contains(chemstring));
                }
            }

            //Sort feature
            ViewData["FormulaNameSort"] = String.IsNullOrEmpty(sortOrder) ? "FormulaNameSort_desc" : "";
            ViewData["ChemIDSort"] = sortOrder == "ChemIDSort" ? "ChemIDSort_desc" : "ChemIDSort";
            ViewData["CASSort"] = sortOrder == "CASSort" ? "CASSort_desc" : "CASSort";
            ViewData["FormulaSort"] = sortOrder == "FormulaSort" ? "FormulaSort_desc" : "FormulaSort";
            ViewData["FormulaWeightSort"] = sortOrder == "FormulaWeightSort" ? "FormulaWeightSort_desc" : "FormulaWeightSort";
            ViewData["HazardSort"] = sortOrder == "HazardSort" ? "HazardSort_desc" : "HazardSort";
            ViewData["StateSort"] = sortOrder == "StateSort" ? "StateSort_desc" : "StateSort";

            switch (sortOrder)
            {
                //Ascending
                case "ChemIDSort":
                    chemicals = chemicals.OrderBy(x => x.ChemID);
                    break;
                case "CASSort":
                    chemicals = chemicals.OrderBy(x => x.CAS);
                    break;
                case "FormulaSort":
                    chemicals = chemicals.OrderBy(x => x.Formula);
                    break;
                case "FormulaWeightSort":
                    chemicals = chemicals.OrderBy(x => x.FormulaWeight);
                    break;
                case "HazardSort":
                    chemicals = chemicals.OrderBy(x => x.Hazard);
                    break;
                case "StateSort":
                    chemicals = chemicals.OrderBy(x => x.State);
                    break;

                //Descending
                case "FormulaNameSort_desc":
                    chemicals = chemicals.OrderByDescending(x => x.FormulaName);
                    break;
                case "ChemIDSort_desc":
                    chemicals = chemicals.OrderByDescending(x => x.ChemID);
                    break;
                case "CASSort_desc":
                    chemicals = chemicals.OrderByDescending(x => x.CAS);
                    break;
                case "FormulaSort_desc":
                    chemicals = chemicals.OrderByDescending(x => x.Formula);
                    break;
                case "FormulaWeightSort_desc":
                    chemicals = chemicals.OrderByDescending(x => x.FormulaWeight);
                    break;
                case "HazardSort_desc":
                    chemicals = chemicals.OrderByDescending(x => x.Hazard);
                    break;
                case "StateSort_desc":
                    chemicals = chemicals.OrderByDescending(x => x.State);
                    break;

                default:
                    chemicals = chemicals.OrderBy(x => x.FormulaName);
                    break;
            }

            return View(await chemicals.ToListAsync());
        }

        // GET: Chemicals/Details/5
        [Authorize(Roles = "Admin,ChemUser,BiologyUser")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemical = await _context.Chemical.SingleOrDefaultAsync(m => m.ChemID == id);
            if (chemical == null)
            {
                return NotFound();
            }

            return View(chemical);
        }

        // GET: Chemicals/Create
        [Authorize(Roles = "Admin,ChemUser,BiologyUser")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Chemicals/Create
        // To protect from overposting attacks, enabled binding of properties
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ChemUser,BiologyUser")]
        public async Task<IActionResult> Create([Bind("ChemID,CAS,CAT,Formula,FormulaName,FormulaWeight,Hazard,SDS,State")] Chemical chemical)
        {
            if (ModelState.IsValid)
            {
                chemical.CAT = "N/A";
                _context.Add(chemical);
                await _context.SaveChangesAsync();
                sp_Logging("2-Change", "Create", "User created a chemical where formula is " + chemical.FormulaName, "Success");
                return RedirectToAction("Index");
            }
            return View(chemical);
        }

        // GET: Chemicals/Edit/5
        [Authorize(Roles = "Admin,ChemUser,BiologyUser")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chemical = await _context.Chemical.SingleOrDefaultAsync(m => m.ChemID == id);
            if (chemical == null)
            {
                return NotFound();
            }
            return View(chemical);
        }

        // POST: Chemicals/Edit/5
        // To protect from overposting attacks, enabled binding of properties
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,ChemUser,BiologyUser")]
        public async Task<IActionResult> Edit(int id, [Bind("ChemID,CAS,CAT,Formula,FormulaName,FormulaWeight,Hazard,SDS,State")] Chemical chemical)
        {
            if (id != chemical.ChemID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chemical);
                    await _context.SaveChangesAsync();
                    sp_Logging("2-Change", "Edit", "User edited a Chemical where ID= " + id.ToString(), "Success");
                }
                catch (DbUpdateConcurrencyException)
                {                   
                    throw;
                }
                return RedirectToAction("Index");
            }
            return View(chemical);
        }

        // GET: Chemicals/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id, bool itemInUse)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (itemInUse)
            {
                var temp = _context.Chemical.First(n => n.ChemID == id);
                ViewData["ItemInUse"] = temp.FormulaName.ToString() + " ID: " + temp.ChemID.ToString();
            }

            var chemical = await _context.Chemical.SingleOrDefaultAsync(m => m.ChemID == id);
            if (chemical == null)
            {
                return NotFound();
            }

            return View(chemical);
        }

        // POST: Chemicals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chemical = await _context.Chemical.SingleOrDefaultAsync(m => m.ChemID == id);
            _context.Chemical.Remove(chemical);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return RedirectToAction("Delete", new { itemInUse = true });
            }
            sp_Logging("3-Remove", "Delete", "User deleted a Chemical where ID=" + id.ToString(), "Success");
            return RedirectToAction("Index");
        }

        private bool ChemicalExists(int? id)
        {
            return _context.Chemical.Any(e => e.ChemID == id);
        }

        //Custom Loggin Solution
        private void sp_Logging(string level, string logger, string message, string exception)
        {
            //Connection string from AppSettings.JSON
            string CS = configuration.GetConnectionString("DefaultConnection");
            //Using Identity middleware to get email address
            string user = User.Identity.Name;
            string app = "Carroll LMS";
            //Subtract 5 hours as the timestamp is in GMT timezone
            DateTime logged = DateTime.Now.AddHours(-5);
            //logged.AddHours(-5);
            string site = "Chemical";
            string query = "insert into dbo.Log([User], [Application], [Logged], [Level], [Message], [Logger], [CallSite]," +
                "[Exception]) values(@User, @Application, @Logged, @Level, @Message,@Logger, @Callsite, @Exception)";
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@User", user);
                cmd.Parameters.AddWithValue("@Application", app);
                cmd.Parameters.AddWithValue("@Logged", logged);
                cmd.Parameters.AddWithValue("@Level", level);
                cmd.Parameters.AddWithValue("@Message", message);
                cmd.Parameters.AddWithValue("@Logger", logger);
                cmd.Parameters.AddWithValue("@Callsite", site);
                cmd.Parameters.AddWithValue("@Exception", exception);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

    }
}
