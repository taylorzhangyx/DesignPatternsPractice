/// ***************************************************************************
/// Copyright (c) 2009, Industrial Logic, Inc., All Rights Reserved.
///
/// This code is the exclusive property of Industrial Logic, Inc. It may ONLY be
/// used by students during Industrial Logic's workshops or by individuals
/// who are being coached by Industrial Logic on a project.
///
/// This code may NOT be copied or used for any other purpose without the prior
/// written consent of Industrial Logic, Inc.
/// ****************************************************************************

//$CopyrightHeader()$

using System;
using NUnit.Framework;

namespace IndustrialLogic.CreationMethod
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
			return new DateTime(year, 11, day);
		}

		[Test]
		public void TermLoanSamePayments()
		{
			DateTime start = November(20, 2003);
			DateTime maturity = November(20, 2006);
			Loan termLoan = new Loan(LOAN_AMOUNT, HIGH_RISK_RATING, maturity);
			termLoan.StartDate = start;
			termLoan.Payment(1000.00, November(20, 2004));
			termLoan.Payment(1000.00, November(20, 2005));
			termLoan.Payment(1000.00, November(20, 2006));
			Assert.AreEqual(2.0, termLoan.Duration, PENNY_PRECISION, "Duration");
			Assert.AreEqual(210.00, termLoan.Capital, PENNY_PRECISION, "Capital");
		}

		[Test]
		public void TermLoanDifferentPayments()
		{
			DateTime start = November(20, 2003);
			DateTime maturity = November(20, 2007);
			Loan termLoan = new Loan(LOAN_AMOUNT, HIGH_RISK_RATING, maturity);
			termLoan.StartDate = start;
			termLoan.Payment(500.00, November(20, 2004));
			termLoan.Payment(1500.00, November(20, 2005));
			termLoan.Payment(1000.00, November(20, 2007));
			Assert.AreEqual(2.5, termLoan.Duration, PENNY_PRECISION, "Duration");
			Assert.AreEqual(262.50, termLoan.Capital, PENNY_PRECISION, "Capital");
		}

		[Test]
		public void TermLoanWithRiskAdjustedCapitalStrategy()
		{
			DateTime start = November(20, 2003);
			DateTime maturity = November(20, 2006);
			double riskAdjustment = 1.25;
			CapitalStrategy riskAdjustedCapitalStrategy = new RiskAdjustedCapitalStrategy(riskAdjustment);
			double outstanding = 0.0;
			Loan termLoan =
				new Loan(riskAdjustedCapitalStrategy, LOAN_AMOUNT, outstanding, HIGH_RISK_RATING, maturity, Loan.NO_DATE);
			termLoan.StartDate = start;
			termLoan.Payment(1000.00, November(20, 2004));
			termLoan.Payment(1000.00, November(20, 2005));
			termLoan.Payment(1000.00, November(20, 2006));
			Assert.AreEqual(2.0, termLoan.Duration, PENNY_PRECISION, "Duration");
			Assert.AreEqual(262.50, termLoan.Capital, PENNY_PRECISION, "Capital");
		}

		[Test]
		public void RevolverNoPayments()
		{
			DateTime start = November(20, 2003);
			DateTime expiry = November(20, 2010);
			Loan revolver = new Loan(LOAN_AMOUNT, HIGH_RISK_RATING, Loan.NO_DATE, expiry);
			revolver.StartDate = start;
			Assert.AreEqual(7.0, revolver.Duration, PENNY_PRECISION, "Duration");
			Assert.AreEqual(609.00, revolver.Capital, PENNY_PRECISION, "Capital");
		}

		[Test]
		public void RevolverWithPayments()
		{
			DateTime start = November(20, 2003);
			DateTime expiry = November(20, 2010);
			Loan revolver = new Loan(LOAN_AMOUNT, HIGH_RISK_RATING, Loan.NO_DATE, expiry);
			revolver.StartDate = start;
			revolver.OutstandingRiskAmount = 1000.00;
			Assert.AreEqual(7.0, revolver.Duration, PENNY_PRECISION, "Duration");
			Assert.AreEqual(651.00, revolver.Capital, PENNY_PRECISION, "Capital");
		}

		[Test]
		public void RCTLNoPaymentsAfterMaturity()
		{
			DateTime start = November(20, 2003);
			DateTime maturity = November(20, 2006);
			DateTime expiry = November(20, 2010);
			double loanAmount = 12000.0;
			Loan revolvingCreditTermLoan =
				new Loan(loanAmount, loanAmount, LOW_RISK_RATING, maturity, expiry);
			revolvingCreditTermLoan.StartDate = start;
			revolvingCreditTermLoan.Payment(500.00, November(20, 2004));
			revolvingCreditTermLoan.Payment(1500.00, November(20, 2005));
			revolvingCreditTermLoan.Payment(1000.00, November(20, 2007));		
			Assert.AreEqual(6.5, revolvingCreditTermLoan.Duration, PENNY_PRECISION, "Duration");
			Assert.AreEqual(585.00, revolvingCreditTermLoan.Capital, PENNY_PRECISION, "Capital");
		}
	}
}
