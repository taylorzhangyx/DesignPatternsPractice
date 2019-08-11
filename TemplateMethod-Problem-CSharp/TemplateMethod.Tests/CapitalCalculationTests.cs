
//
/// <summary>
/// ***************************************************************************
/// Copyright (c) 2005, Industrial Logic, Inc., All Rights Reserved.
///
/// This code is the exclusive property of Industrial Logic, Inc. It may ONLY be
/// used by students during Industrial Logic's workshops or by individuals
/// who are being coached by Industrial Logic on a project.
///
/// This code may NOT be copied or used for any other purpose without the prior
/// written consent of Industrial Logic, Inc.
/// ****************************************************************************
/// </summary>

using System;
using NUnit.Framework;

namespace IndustrialLogic.Strategy.Tests
{
	[TestFixture]
	public class CapitalCalculationTests
	{
		private const double LOAN_AMOUNT = 3000.0;
		private const double PENNY_PRECISION = 0.01;
		private const int HIGH_RISK_RATING = 4;
		private const int LOW_RISK_RATING = 1;

		private DateTime November(int day, int year)
		{
			return (new DateTime (year, 11, day));
		}

		[Test]
		public void TermLoanSamePayments()
		{
			DateTime start = November(20, 2003);
			DateTime maturity = November(20, 2006);
			Loan termLoan = Loan.NewTermLoan(LOAN_AMOUNT, start, maturity, HIGH_RISK_RATING);
			termLoan.payment(1000.00, November(20, 2004));
			termLoan.payment(1000.00, November(20, 2005));
			termLoan.payment(1000.00, November(20, 2006));
			Assert.AreEqual(2.0, termLoan.Duration(), PENNY_PRECISION, "duration");
			Assert.AreEqual(210.00, termLoan.Capital(), PENNY_PRECISION, "capital");
		}

		[Test]
		public void TermLoanDifferentPayments()
		{
			DateTime start = November(20, 2003);
			DateTime maturity = November(20, 2006);
			Loan termLoan = Loan.NewTermLoan(LOAN_AMOUNT, start, maturity, HIGH_RISK_RATING);
			termLoan.payment(500.00, November(20, 2004));
			termLoan.payment(1500.00, November(20, 2005));
			termLoan.payment(1000.00, November(20, 2006));
			Assert.AreEqual (2.16, termLoan.Duration(), PENNY_PRECISION, "duration");
			Assert.AreEqual (227.50, termLoan.Capital(), PENNY_PRECISION, "capital");
		}

		[Test]
		public void RevolverNoPayments()
		{
			DateTime start = November(20, 2003);
			DateTime expiry = November(20, 2010);
			Loan revolver = Loan.NewRevolver(LOAN_AMOUNT, start, expiry, HIGH_RISK_RATING);
			Assert.AreEqual(7.0, revolver.Duration(), PENNY_PRECISION, "duration");
			Assert.AreEqual(609.00, revolver.Capital(), PENNY_PRECISION, "capital");
		}

		[Test]
		public void RevolverWithPayments()
		{
			DateTime start = November(20, 2003);
			DateTime expiry = November(20, 2010);
			Loan revolver = Loan.NewRevolver(LOAN_AMOUNT, start, expiry, HIGH_RISK_RATING);
			revolver.OutstandingRiskAmount = 1000.00;
			Assert.AreEqual(7.0, revolver.Duration(), PENNY_PRECISION, "duration");
			Assert.AreEqual(651.00, revolver.Capital(), PENNY_PRECISION, "capital");
		}

		[Test]
		public void AdvisedLineMustHaveLowRiskRating()
		{
			DateTime start = November(20, 2003);
			DateTime expiry = November(20, 2010);
			Loan advisedLine = Loan.NewAdvisedLine(LOAN_AMOUNT, start, expiry, HIGH_RISK_RATING);
			Assert.IsNull (advisedLine, "null loan because not at right risk level");
		}

		[Test]
		public void AdvisedLine()
		{
			DateTime start = November(20, 2003);
			DateTime expiry = November(20, 2010);
			Loan advisedLine = Loan.NewAdvisedLine(LOAN_AMOUNT, start, expiry, LOW_RISK_RATING);
			Assert.AreEqual(7.0, advisedLine.Duration(), PENNY_PRECISION, "duration");
			Assert.AreEqual(15.75, advisedLine.Capital(), PENNY_PRECISION, "capital");
		}
	}
}
