
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
using System.Collections;

namespace IndustrialLogic.Strategy
{
	public class Loan
	{
		/// Use NO_DATE to indicate that no date is set (the equivalent of 'null Date' in Java)
		public static readonly DateTime NO_DATE = DateTime.MaxValue;

		private double commitment;
		private double outstanding;
		private DateTime start;
		private DateTime maturity;
		private DateTime expiry;
		private int riskRating;
		private double unusedPercentage;
		private IList payments = new ArrayList();
		private CapitalStrategy capitalStrategy;

		protected internal class Payment
		{
			private double   amount;
			private DateTime date;

			public Payment(double amount, DateTime date)
			{
				this.amount = amount;
				this.date = date;
			}

			public double Amount
			{
				get {
					return this.amount;
				}
			}

			public DateTime Date
			{
				get {
					return this.date;
				}
			}
		}

		private Loan(double commitment, double outstanding,
					 DateTime start, DateTime expiry, DateTime maturity, int riskRating,
					 CapitalStrategy capitalStrategy)
		{
			this.commitment = commitment;
			this.outstanding = outstanding;
			this.start = start;
			this.expiry = expiry;
			this.maturity = maturity;
			this.riskRating = riskRating;
			this.capitalStrategy =  capitalStrategy;
			UnusedPercentage = 1.0;
		}

		public static Loan NewTermLoan(double commitment, DateTime start,
									   DateTime maturity, int riskRating)
		{
			return new Loan(commitment, commitment, start, NO_DATE, maturity, riskRating,
							new CapitalStrategyTermLoan());
		}

		public static Loan NewRevolver(double commitment, DateTime start,
									   DateTime expiry, int riskRating)
		{
			return new Loan(commitment, 0, start, expiry, NO_DATE, riskRating,
							new CapitalStrategyRevolver());
		}

		public static Loan NewAdvisedLine(double commitment, DateTime start,
										  DateTime expiry, int riskRating)
		{
			if (riskRating > 3)
				return null;
			Loan advisedLine = new Loan(commitment, (double) 0, start, expiry, NO_DATE, riskRating,
										(CapitalStrategy) new CapitalStrategyAdvisedLine());
			advisedLine.UnusedPercentage = 0.1;
			return advisedLine;
		}

		public double Capital()
		{
			return capitalStrategy.Capital(this);
		}

		public double OutstandingRiskAmount {
			get {
				return outstanding;
			}
			set {
				outstanding = value;
			}
		}

		public double UnusedRiskAmount()
		{
			return (commitment - OutstandingRiskAmount);
		}

		public double Duration()
		{
			return capitalStrategy.Duration(this);
		}

		public void payment(double paymentAmount, DateTime paymentDate)
		{
			Payment payment = new Payment(paymentAmount, paymentDate);
			OutstandingRiskAmount -= payment.Amount;
			payments.Add(payment);
		}

		public double UnusedPercentage
		{
			get {
				return unusedPercentage;
			}
			set {
				unusedPercentage = value;
			}
		}

		public DateTime Expiry
		{
			get {
				return expiry;
			}
		}

		public DateTime Maturity
		{
			get {
				return maturity;
			}
		}

		public double Commitment
		{
			get {
				return commitment;
			}
		}

		public int RiskRating
		{
			get {
				return riskRating;
			}
		}

		public DateTime Start
		{
			get {
				return start;
			}
		}

		internal IList Payments
		{
			get {
				return payments;
			}
		}
	}
}
