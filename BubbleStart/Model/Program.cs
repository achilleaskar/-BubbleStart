﻿using System;

namespace BubbleStart.Model
{
    public class Program : BaseModel
    {
        #region Fields

        private float _Amount;
        private DateTime _DayOfIssue;
        private ProgramTypes _ProgramType;
        private DateTime _StartDay;

        #endregion Fields



        private int _Months;


        public int Months
        {
            get
            {
                return _Months;
            }

            set
            {
                if (_Months == value)
                {
                    return;
                }

                _Months = value;
                RaisePropertyChanged();
            }
        }
        #region Enums

        public enum ProgramTypes
        {
            ReformerPilates,
            Pilates,
            Functional,
            PilatesFunctional,
            freeUse,
            MedicalExersise
        }

        public override string ToString()
        {
            switch (ProgramType)
            {
                case ProgramTypes.ReformerPilates:
                    return "Reformer Pilates";

                case ProgramTypes.Pilates:
                    return "Pilates";

                case ProgramTypes.Functional:
                    return "Functional";

                case ProgramTypes.PilatesFunctional:
                    return "Pilates & Functional";

                case ProgramTypes.freeUse:
                    return "Ελέυθερη Χρήση";
                case ProgramTypes.MedicalExersise:
                    return "Medical Exercise";
            }
            return "Ανενεργό";
        }

        #endregion Enums

        #region Properties

        public float Amount
        {
            get
            {
                return _Amount;
            }

            set
            {
                if (_Amount == value)
                {
                    return;
                }

                _Amount = value;
                RaisePropertyChanged();
            }
        }

        public DateTime DayOfIssue
        {
            get
            {
                return _DayOfIssue;
            }

            set
            {
                if (_DayOfIssue == value)
                {
                    return;
                }
                if (value == null)
                {
                    RaisePropertyChanged();
                    return;
                }

                _DayOfIssue = value;
                RaisePropertyChanged();
            }
        }




        private int _Showups;


        public int Showups
        {
            get
            {
                return _Showups;
            }

            set
            {
                if (_Showups == value)
                {
                    return;
                }

                _Showups = value;
                RaisePropertyChanged();
            }
        }


        public ProgramTypes ProgramType
        {
            get
            {
                return _ProgramType;
            }

            set
            {
                if (_ProgramType == value)
                {
                    return;
                }

                _ProgramType = value;
                RaisePropertyChanged();
            }
        }

        public DateTime StartDay
        {
            get
            {
                return _StartDay;
            }

            set
            {
                if (_StartDay == value)
                {
                    return;
                }
                if (value == null)
                {
                    RaisePropertyChanged();
                    return;
                }

                _StartDay = value;
                RaisePropertyChanged();
            }
        }

        #endregion Properties
    }
}