using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TransactionManagement.Data;
using TransactionManagement.Models;
using TransactionManagement.Service;

namespace TransactionManagement.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService userService;

        public TransactionsController(ApplicationDbContext context,IUserService userService)
        {
            _context = context;
            this.userService = userService;
        }

        // GET: Transactions
        public async Task<IActionResult> Index(bool? Type)
        {
            
            // var Transaction = _context.Transactions.Where(owner => owner.OwnerId == userService.GetUserId()).ToListAsync();
            var user = _context.UserDetails.
                Where(id => id.OwnerId == userService.GetUserId())
                .FirstOrDefault();
            ViewData["user"] = user;
            if (Type != null)
            {
                //var data = _context.Transactions.
                //    Where(uid => uid.OwnerId == userService.GetUserId()).ToListAsync();
                //var list = new List<Transaction>();

                return View(
                    await _context.Transactions.
                    Where(type => type.Type == Type 
                    &&
                    type.OwnerId==userService.GetUserId())
                    .ToListAsync()
                );
            }
            return View(
                await _context.Transactions
                .Where(owner => owner.OwnerId == userService.GetUserId())
                .ToListAsync()
                );
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }
            if (transaction.OwnerId == userService.GetUserId())
            {
                return View(transaction);
            }
           
            return Forbid();
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Type,ShortNote,Amount,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                var UserDetail = _context.UserDetails.
                    Where(user => user.OwnerId == userService.GetUserId()).
                    FirstOrDefault();
                if (UserDetail == null)
                {
                    UserDetails userDetails = new UserDetails();
                    userDetails.OwnerId = userService.GetUserId();
                    if (transaction.Type == true)
                    {
                        userDetails.TotalIncome = transaction.Amount;
                        userDetails.TotalExpence = 0;
                    }
                    else
                    {
                        userDetails.TotalExpence = transaction.Amount;
                        userDetails.TotalIncome = 0;
                    }
                    userDetails.TotalBalance = userDetails.TotalIncome - userDetails.TotalExpence;

                    _context.UserDetails.Add(userDetails);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    if (transaction.Type == true)
                    {
                        UserDetail.TotalIncome = UserDetail.TotalIncome + transaction.Amount;
                    }
                    else
                    {
                        UserDetail.TotalExpence = UserDetail.TotalExpence + transaction.Amount;
                    }

                    UserDetail.TotalBalance = UserDetail.TotalIncome - UserDetail.TotalExpence;
                    _context.UserDetails.Update(UserDetail);
                    await _context.SaveChangesAsync();
                }

                transaction.OwnerId = userService.GetUserId();
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);



            if (transaction == null)
            {
                return NotFound();
            }
            if (transaction.OwnerId != userService.GetUserId())
            {
                return Forbid();
            }
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OwnerId,Type,ShortNote,Date,Amount")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var preTransaction = await _context.Transactions.FindAsync(id);
                    var UserDetail = _context.UserDetails.
                        Where(user => user.OwnerId == userService.GetUserId()).
                        FirstOrDefault();

                    if (preTransaction.Type == false)
                    {
                        UserDetail.TotalExpence = UserDetail.TotalExpence - preTransaction.Amount;
                    }
                    else
                    {
                        UserDetail.TotalIncome = UserDetail.TotalIncome - preTransaction.Amount;
                    }

                    UserDetail.TotalBalance = UserDetail.TotalIncome - UserDetail.TotalExpence;

                    if (transaction.Type == true)
                    {
                        UserDetail.TotalIncome = UserDetail.TotalIncome + transaction.Amount;
                    }
                    else
                    {
                        UserDetail.TotalExpence = UserDetail.TotalExpence + transaction.Amount;
                    }

                    UserDetail.TotalBalance = UserDetail.TotalIncome - UserDetail.TotalExpence;
                    _context.UserDetails.Update(UserDetail);
                    await _context.SaveChangesAsync();
                }
                catch
                {

                }
               


                try
                {
                    var transactions = await _context.Transactions
                        .FirstOrDefaultAsync(m => m.Id == id);
                    transactions.Id = transaction.Id;
                    transactions.OwnerId = transaction.OwnerId;
                    transactions.Type = transaction.Type;
                    transactions.ShortNote = transaction.ShortNote;
                    transactions.Date = transaction.Date;
                    transactions.Amount = transaction.Amount;
                    _context.Update(transactions);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
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
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            if (transaction.OwnerId != userService.GetUserId())
            {
                return Forbid();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id != null)
            {
                var t = await _context.Transactions
                .FirstOrDefaultAsync(m => m.Id == id);

                if (t != null)
                {
                    var UserDetail = _context.UserDetails.
                            Where(user => user.OwnerId == userService.GetUserId()).
                            FirstOrDefault();

                    if (t.Type == false)
                    {
                        UserDetail.TotalExpence = UserDetail.TotalExpence - t.Amount;
                    }
                    else
                    {
                        UserDetail.TotalIncome = UserDetail.TotalIncome - t.Amount;
                    }

                    UserDetail.TotalBalance = UserDetail.TotalIncome - UserDetail.TotalExpence;
                    _context.UserDetails.Update(UserDetail);
                    await _context.SaveChangesAsync();
                }
            }

            var transaction = await _context.Transactions.FindAsync(id);
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
}
