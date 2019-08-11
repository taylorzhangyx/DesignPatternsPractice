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
using System.Collections;

namespace IndustrialLogic.CreationMethod
{
	public class Loan
	{
		/// Use NO_DATE to indicate that no date is set (the equivalent of 'null Date' in Java)
		public static readonly DateTime NO_DATE = DateTime.MaxValue;

		private double commitment;
		private double outstanding;
		private int riskRating;
		private DateTime maturity;
		private DateTime expiry;
		private DateTime start;
		private DateTime today = NO_DATE;
		private IList payments = new ArrayList();
		private double unusedPercentage;
		private CapitalStrategy capitalStrategy;

		public Loan(double commitment, int riskRating, DateTime maturity) :
			this(commitment, 0.00, riskRating, maturity, NO_DATE)
		{
		}

		public Loan(double commitment, int riskRating, DateTime maturity, DateTime expiry) :
			this(commitment, 0.00, riskRating, maturity, expiry)
		{
		}

		public Loan(double commitment, double outstanding,
		            int customerRating, DateTime maturity, DateTime expiry) :
		            	this(null, commitment, outstanding, customerRating, maturity, expiry)
		{
		}

		public Loan(CapitalStrategy capitalStrategy, double commitment,
		            int riskRating, DateTime maturity, DateTime expiry) :
		            	this(capitalStrategy, commitment, 0.00, riskRating, maturity, expiry)
		{
		}

		public Loan(CapitalStrategy capitalStrategy, double commitment,
		            double outstanding, int riskRating,
		            DateTime maturity, DateTime expiry)
		{
			this.commitment = commitment;
			this.outstanding = outstanding;
			this.riskRating = riskRating;
			this.maturity = maturity;
			this.expiry = expiry;
			this.capitalStrategy = capitalStrategy;

			if (capitalStrategy == null)
			{
				if (expiry == NO_DATE)
					this.capitalStrategy = new CapitalStrategyTermLoan();
				else if (maturity == NO_DATE)
					this.capitalStrategy = new CapitalStrategyRevolver();
				else
					this.capitalStrategy = new CapitalStrategyRCTL();
			}
		}

		public double Capital
		{
			get { return capitalStrategy.Capital(this); }
		}

		public double OutstandingRiskAmount
		{
			get { return outstanding; }
			set { this.outstanding = value; }
		}

		internal double UnusedRiskAmount
		{
			get { return (Commitment - OutstandingRiskAmount); }
		}

		public double Duration
		{
			get { return capitalStrategy.Duration(this); }
		}

		public double UnusedPercentage
		{
			get { return unusedPercentage; }
			set { this.unusedPercentage = value; }
		}

		internal DateTime Expiry
		{
			get { return expiry; }
		}

		internal DateTime Maturity
		{
			get { return maturity; }
		}

		internal double Commitment
		{
			get { return commitment; }
		}

		internal int RiskRating
		{
			get { return riskRating; }
		}

		public DateTime StartDate
		{
			get { return start; }
			set { this.start = value; }
		}

		internal DateTime Today
		{
			get { return today; }
		}

		internal IList Payments
		{
			get { return payments; }
		}

		public void Payment(double paymentAmount, DateTime paymentDate)
		{
			Payment payment = new Payment(paymentAmount, paymentDate);
			outstanding = outstanding - payment.Amount;
			payments.Add(payment);
		}

		public void SetStrategy(CapitalStrategy strategy)
		{
			this.capitalStrategy = strategy;
		}
	}
}
