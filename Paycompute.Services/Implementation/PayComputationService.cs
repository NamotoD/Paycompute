using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Paycompute.Entity;
using Paycompute.Persistence;

namespace Paycompute.Services.Implementation
{
    public class PayComputationService : IPayComputationService
    {
        private decimal contractualEarnings;
        private readonly ApplicationDbContext _context;
        private decimal overtimeHours;

        public PayComputationService(ApplicationDbContext context)
        {
            _context = context;
        }
        public decimal ContractualEarnings(decimal contractualHours, decimal hoursWorked, decimal hourlyRate)
        {
            if(hoursWorked < contractualHours)
            {
                contractualEarnings = hoursWorked * hourlyRate;
            }
            else
            {
                contractualEarnings = contractualHours * hourlyRate;
            }
            return contractualEarnings;
        }

        public async Task CreateAsync(PaymentRecord paymentRecord)
        {
            await _context.PaymentRecords.AddRangeAsync(paymentRecord);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<PaymentRecord> GetAll() => _context.PaymentRecords.OrderBy(p => p.EmployeeId);

        public IEnumerable<SelectListItem> GetAllTaxYear()
        {
            var allTaxYear = _context.TaxYears.Select(TaxYears => new SelectListItem
            { 
                Text = TaxYears.YearOfTax,
                Value = TaxYears.Id.ToString()
            });
            return allTaxYear;
        }

        public PaymentRecord GetById(int id) => 
            _context.PaymentRecords.Where(pay => pay.Id == id).FirstOrDefault();

        public decimal NetPay(decimal totalEarnings, decimal totalDeduction) =>
            totalEarnings - totalDeduction;

        public decimal OvertimeEarnings(decimal overtimeRate, decimal overtimeHours) => 
            overtimeRate * overtimeHours;

        public decimal OvertimeHours(decimal hoursWorked, decimal contractulHours)
        {
            if (hoursWorked <= contractulHours)
            {
                overtimeHours = 0.00m;
            }
            else if (hoursWorked > contractulHours)
            {
                overtimeHours = hoursWorked - contractulHours;
            }
            return overtimeHours;
        }

        public decimal OvertimeRate(decimal hourlyRate) => 
            hourlyRate * 1.5m;

        public decimal TotalDeduction(decimal tax, decimal nic, decimal studentLoanRepayment, decimal unionFees) =>
            tax + nic + studentLoanRepayment + unionFees;

        public decimal TotalEarnings(decimal overtimeEarnings, decimal contractualEarnings) =>
            overtimeEarnings + contractualEarnings;
    }
}
