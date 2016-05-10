﻿using NUnit.Framework;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using gzDAL.Conf;
using gzDAL.DTO;
using gzDAL.Models;
using gzDAL.Repos;
using Microsoft.AspNet.Identity;
using Assert = NUnit.Framework.Assert;
using System.Collections.Generic;

namespace gzWeb.Tests.Models {
    [TestFixture]
    public class InvestmentTests {

        [OneTimeSetUp]
        public void Setup() {
            Database.SetInitializer<ApplicationDbContext>(null);
        }

        public void CreateTestCustomerPortfolioSelections(int custId) {
            var db = new ApplicationDbContext(null);
            var cpRepo = new CustPortfolioRepo(db);

            /*** For phase I we test only single portfolio selections ***/
            //// Jan 2015
            cpRepo.SaveDbCustMonthsPortfolioMix(custId, RiskToleranceEnum.Low, 100, 2015, 1, new DateTime(2015, 1, 1));
            //await cpRepo.SetCustMonthsPortfolio(custId, RiskToleranceEnum.Medium, 50, 2015, 1, new DateTime(2015, 1, 1));
            //await cpRepo.SetCustMonthsPortfolio(custId, RiskToleranceEnum.High, 20, 2015, 1, new DateTime(2015, 1, 1));
            //// Feb
            //await cpRepo.SetCustMonthsPortfolio(custId, RiskToleranceEnum.Low, 10, 2015, 2, new DateTime(2015, 2, 1));
            //await cpRepo.SetCustMonthsPortfolio(custId, RiskToleranceEnum.Medium, 50, 2015, 2, new DateTime(2015, 2, 1));
            //await cpRepo.SetCustMonthsPortfolio(custId, RiskToleranceEnum.High, 40, 2015, 2, new DateTime(2015, 2, 1));
            // Mar
            cpRepo.SaveDbCustMonthsPortfolioMix(custId, RiskToleranceEnum.High, 100, 2015, 3, new DateTime(2015, 3, 1));
            //// Apr
            //await cpRepo.SetCustMonthsPortfolio(custId, RiskToleranceEnum.Low, 30, 2015, 4, new DateTime(2015, 4, 1));
            //await cpRepo.SetCustMonthsPortfolio(custId, RiskToleranceEnum.Medium, 30, 2015, 4, new DateTime(2015, 4, 1));
            //await cpRepo.SetCustMonthsPortfolio(custId, RiskToleranceEnum.High, 40, 2015, 4, new DateTime(2015, 4, 1));
            //// May
            //await cpRepo.SetCustMonthsPortfolio(custId, RiskToleranceEnum.Low, 10, 2015, 5, new DateTime(2015, 5, 1));
            //await cpRepo.SetCustMonthsPortfolio(custId, RiskToleranceEnum.Medium, 20, 2015, 5, new DateTime(2015, 5, 1));
            //await cpRepo.SetCustMonthsPortfolio(custId, RiskToleranceEnum.High, 70, 2015, 5, new DateTime(2015, 5, 1));
            // Jun
            cpRepo.SaveDbCustMonthsPortfolioMix(custId, RiskToleranceEnum.Medium, 100, 2015, 6, new DateTime(2015, 6, 1));
        }

        [Test]
        public void SaveDbSellPortfolio() {

            using (var db = new ApplicationDbContext(null)) {

                var custId = CreateTestCustomer(db, TestUser6Month(db));

                // Last Day of present Month @ 23:00
                var yearCurrent = DateTime.UtcNow.Year;
                var monthCurrent = DateTime.UtcNow.Month;
                var lastMonthDay = new DateTime(yearCurrent, monthCurrent, DateTime.DaysInMonth(yearCurrent, monthCurrent),
                    23, 00, 00);

                var soldShares = new InvBalanceRepo(db, new CustFundShareRepo(db), new GzTransactionRepo(db))
                    .SaveDbSellCustomerPortfolio(custId, lastMonthDay);

                Console.WriteLine("SaveDbSellPortfolio() returned soldShares: " + soldShares);

            }
        }

        [Test]
        public void SaveDbUpdCustomerBalancesByTrx() {

            using (var db = new ApplicationDbContext(null)) {

                CreateTestCustomer(db, TestUser6Month(db));
                CreateTestCustomer(db, TestInfo(db));
                CreateTestCustomer(db, TestUser(db));

                var custIdList = db.Users
                    .Where(u => new List<string>() {

                        "6month@allocation.com",
                        "info@nessos.gr",
                        "testuser@gz.com"

                    }.Contains(u.Email)).Select(u=>u.Id).ToList();

                foreach (var custId in custIdList) {

                    // Add invested Customer Portfolio
                    CreateTestCustomerPortfolioSelections(custId);

                    CreateTestPlayerLossTransactions(custId);
                    CreateTestPlayerDepositWidthdrawnTransactions(custId);

                    new InvBalanceRepo(db, new CustFundShareRepo(db), new GzTransactionRepo(db))
                        .SaveDbCustomerMonthlyBalancesByTrx(custId);
                }
            }
        }

        [Test]
        public void SaveDbUpdEveryMatrixUserBalancesByTrx() {

            string[] customerEmails = new string[] { "info@nessos.gr", "testuser@gz.com" };

            using (ApplicationDbContext context = new ApplicationDbContext(null)) {

                foreach (var customerEmail in customerEmails) {

                    var custId = context.Users.Where(u => u.Email == customerEmail).Select(u => u.Id).Single();

                    // Add invested Customer Portfolio
                    CreateTestCustomerPortfolioSelections(custId);

                    CreateTestPlayerLossTransactions(custId);
                    CreateTestPlayerDepositWidthdrawnTransactions(custId);

                    new InvBalanceRepo(context, new CustFundShareRepo(context), new GzTransactionRepo(context))
                        .SaveDbCustomerMonthlyBalancesByTrx(custId);

                }
            }
        }

        public void CreateTestPlayerDepositWidthdrawnTransactions(int custId) {
            using (var db = new ApplicationDbContext(null)) {
                var gzTrx = new GzTransactionRepo(db);

                // Add deposit, withdrawals
                gzTrx.SaveDbTransferToGamingAmount(custId, 50, new DateTime(2015, 4, 30));
                gzTrx.SaveDbGzTransaction(customerId: custId, gzTransactionType: GzTransactionJournalTypeEnum.Deposit,
                    amount: 100, createdOnUtc: new DateTime(2015, 7, 15));
                gzTrx.SaveDbInvWithdrawalAmount(custId, 30, new DateTime(2015, 5, 30));
                gzTrx.SaveDbTransferToGamingAmount(custId, 40, new DateTime(2015, 5, 31));
                gzTrx.SaveDbInvWithdrawalAmount(custId, 40, new DateTime(2015, 6, 1));
                gzTrx.SaveDbTransferToGamingAmount(custId, 20, new DateTime(2015, 6, 15));
            }
        }

        public void CreateTestPlayerLossTransactions(int custId) {
            using (var db = new ApplicationDbContext(null)) {
                var gzTrx = new GzTransactionRepo(db);

                // Add playing losses for first 6 months of 2015
                gzTrx.SaveDbPlayingLoss(customerId: custId, totPlayinLossAmount: 160, creditPcnt: 50,
                    createdOnUtc: new DateTime(2015, 1, 1));
                gzTrx.SaveDbPlayingLoss(customerId: custId, totPlayinLossAmount: 120, creditPcnt: 50,
                    createdOnUtc: new DateTime(2015, 2, 1));
                gzTrx.SaveDbPlayingLoss(customerId: custId, totPlayinLossAmount: 200, creditPcnt: 50,
                    createdOnUtc: new DateTime(2015, 3, 1));
                gzTrx.SaveDbPlayingLoss(customerId: custId, totPlayinLossAmount: 170, creditPcnt: 50,
                    createdOnUtc: new DateTime(2015, 4, 1));
                gzTrx.SaveDbPlayingLoss(customerId: custId, totPlayinLossAmount: 300, creditPcnt: 50,
                    createdOnUtc: new DateTime(2015, 5, 1));
                gzTrx.SaveDbPlayingLoss(customerId: custId, totPlayinLossAmount: 200, creditPcnt: 50,
                    createdOnUtc: new DateTime(2015, 6, 1));
                gzTrx.SaveDbPlayingLoss(customerId: custId, totPlayinLossAmount: 200, creditPcnt: 50,
                    createdOnUtc: new DateTime(2016, 1, 1));
                gzTrx.SaveDbPlayingLoss(customerId: custId, totPlayinLossAmount: 100, creditPcnt: 50,
                    createdOnUtc: new DateTime(2015, 9, 30));
            }
        }

        [Test]
        public void SaveDbUpdAllCustomerBalances() {
            using (var db = new ApplicationDbContext(null)) {
                new InvBalanceRepo(db, new CustFundShareRepo(db), new GzTransactionRepo(db))
                    .SaveDbAllCustomersMonthlyBalances();
            }
        }

        private int CreateTestCustomer(ApplicationDbContext db, ApplicationUser newUser) {

            db.Users.AddOrUpdate(c => new { c.Email }, newUser);
            db.SaveChanges();

            var custId = db.Users.Where(u => u.Email == newUser.Email).Select(u => u.Id).Single();
            return custId;
        }

#region TestUsers

        private ApplicationUser TestInfo(ApplicationDbContext db) {

            var manager = new ApplicationUserManager(new CustomUserStore(db),
                new DataProtectionProviderFactory(() => null));

            var newUser = new ApplicationUser {
                UserName = "infonesos",
                Email = "info@nessos.gr",
                FirstName = "Info",
                LastName = "Nessos",
                Birthday = new DateTime(1975, 10, 13),
                Currency = "EUR",
                PasswordHash = manager.PasswordHasher.HashPassword("gz2016!@"),
                EmailConfirmed = true
            };
            return newUser;
        }

        private ApplicationUser TestUser6Month(ApplicationDbContext db) {

            var manager = new ApplicationUserManager(new CustomUserStore(db),
                new DataProtectionProviderFactory(() => null));

            var newUser = new ApplicationUser() {
                UserName = "6month@allocation.com",
                Email = "6month@allocation.com",
                EmailConfirmed = true,
                FirstName = "Six",
                LastName = "Month",
                Birthday = new DateTime(1990, 1, 1),
                Currency = "EUR",
                PasswordHash = manager.PasswordHasher.HashPassword("1q2w3e")
            };
            return newUser;
        }

        private ApplicationUser TestUser(ApplicationDbContext db) {

            var manager = new ApplicationUserManager(new CustomUserStore(db),
                new DataProtectionProviderFactory(() => null));

            var newUser = new ApplicationUser {
                UserName = "testuser",
                Email = "testuser@gz.com",
                FirstName = "test",
                LastName = "user",
                Birthday = new DateTime(1975, 10, 13),
                Currency = "EUR",
                PasswordHash = manager.PasswordHasher.HashPassword("gz2016!@"),
                EmailConfirmed = true
            };
            return newUser;
        }

#endregion

    }
}
