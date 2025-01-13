﻿using BubbleStart.Helpers;
using DocumentFormat.OpenXml.Wordprocessing;
using EnumsNET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows.Media;
using static BubbleStart.Model.Program;

namespace BubbleStart.Model
{
    public class ShowUp : BaseModel
    {
        private bool _IsSelected;

        private bool _Is30min;

        private bool _Present;

        private bool _Test;

        //private int _GymNum;

        //public int GymNum
        //{
        //    get
        //    {
        //        return _GymNum;
        //    }

        //    set
        //    {
        //        if (_GymNum == value)
        //        {
        //            return;
        //        }

        //        _GymNum = value;
        //        RaisePropertyChanged();
        //    }
        //}

        private User _Gymnast;

        public User Gymnast
        {
            get
            {
                return _Gymnast;
            }

            set
            {
                if (_Gymnast == value)
                {
                    return;
                }

                _Gymnast = value;
                RaisePropertyChanged();
            }
        }

        public bool Test
        {
            get
            {
                return _Test;
            }

            set
            {
                if (_Test == value)
                {
                    return;
                }

                _Test = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(RealColorOnl));
            }
        }

        public string SecBodyPartsDesc => GetSecBodyPartsDesc();

        private string GetSecBodyPartsDesc()
        {
            if (string.IsNullOrWhiteSpace(SecBodyPartsString))
            {
                return "";
            }
            return string.Join(", ", SecBodyPartsString.Split(new[] { ',' }).Select(r => StaticResources.GetDescription((SecBodyPart)int.Parse(r))));
        }

        private string _SecBodyPartsString;

        public string SecBodyPartsString
        {
            get
            {
                return _SecBodyPartsString;
            }

            set
            {
                if (_SecBodyPartsString == value)
                {
                    return;
                }

                _SecBodyPartsString = value;
                RaisePropertyChanged();
            }
        }

        private BodyPart _BodyPart;

        public BodyPart BodyPart
        {
            get
            {
                return _BodyPart;
            }

            set
            {
                if (_BodyPart == value)
                {
                    return;
                }

                _BodyPart = value;
                RaisePropertyChanged();
                Customer?.RaisePropertyChanged(nameof(Customer.LastPart));
            }
        }

        public bool Present
        {
            get
            {
                return _Present;
            }

            set
            {
                if (_Present == value)
                {
                    return;
                }

                _Present = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(RealColor));
            }
        }

        public bool Is30min
        {
            get
            {
                return _Is30min;
            }

            set
            {
                if (_Is30min == value)
                {
                    return;
                }

                _Is30min = value;
                RaisePropertyChanged();
            }
        }

        public IList<Change> Changes { get; set; }

        [NotMapped]
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }

            set
            {
                if (_IsSelected == value)
                {
                    return;
                }

                _IsSelected = value;
                RaisePropertyChanged();
            }
        }

        public string Type => GetShowUpType();

        private string GetShowUpType()
        {
            if (ProgramModeNew == ProgramMode.online)
            {
                return "Onl";
            }
            else if (ProgramModeNew == ProgramMode.outdoor)
            {
                return "Out";
            }
            else
                return "";
        }

        public SolidColorBrush RealColor => Present ? new SolidColorBrush(Colors.LightPink) : Real ? new SolidColorBrush(Colors.Transparent) : new SolidColorBrush(Colors.OrangeRed);
        public SolidColorBrush RealColorOnl => Test ? new SolidColorBrush(Colors.LightBlue) : new SolidColorBrush(Colors.Transparent);

        private bool _Real = true;

        public bool Real
        {
            get => _Real;

            set
            {
                if (_Real == value)
                {
                    return;
                }

                _Real = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(RealColor));
            }
        }

        private int _Count;

        [NotMapped]
        public int Count
        {
            get => _Count;

            set
            {
                if (_Count == value)
                {
                    return;
                }

                _Count = value;
                RaisePropertyChanged();
            }
        }

        #region Fields

        private bool _Arrive;
        private SolidColorBrush _Color;
        private DateTime _Left;
        private DateTime _Time;

        private bool _Massage;

        public bool Massage
        {
            get => _Massage;

            set
            {
                if (_Massage == value)
                {
                    return;
                }

                _Massage = value;
                RaisePropertyChanged();
            }
        }

        private ProgramMode _ProgramModeNew;

        public ProgramMode ProgramModeNew
        {
            get
            {
                return _ProgramModeNew;
            }

            set
            {
                if (_ProgramModeNew == value)
                {
                    return;
                }

                _ProgramModeNew = value;
                RaisePropertyChanged();
            }
        }

        private ProgramMode _ProgramMode;

        public ProgramMode ProgramMode // secondary program mode. For Pilates - Functional - Yoga
        {
            get
            {
                return _ProgramMode;
            }

            set
            {
                if (_ProgramMode == value)
                {
                    return;
                }

                _ProgramMode = value;
                RaisePropertyChanged();
            }
        }

        #endregion Fields

        #region Properties

        public bool Arrive
        {
            get => _Arrive;

            set
            {
                if (_Arrive == value)
                {
                    return;
                }

                _Arrive = value;
                RaisePropertyChanged();
            }
        }

        private Program _Prog;

        public Program Prog
        {
            get => _Prog;

            set
            {
                if (_Prog == value)
                {
                    return;
                }

                _Prog = value;
                RaisePropertyChanged();
            }
        }

        public string Description => GetDescription();
        public DateTime AppointmentTime => GetAppointmentTime();

        private DateTime GetAppointmentTime()
        {
            var daysAppos = Customer.apps?.Where(d => d.DateTime.Date == Arrived.Date.Date).ToList();
            return daysAppos?.FirstOrDefault(a => a.Room == GetProgramModeToRoom(0) || a.Room == GetProgramModeToRoom(2))?.DateTime ?? daysAppos?.FirstOrDefault()?.DateTime ?? Arrived;
        }

        internal RoomEnum GetProgramModeToRoom(int v)
        {
            if (v == 2)
            {
                switch (ProgramModeNew)
                {
                    case ProgramMode.functional:
                        return RoomEnum.Fitness;

                    case ProgramMode.massage:
                        return RoomEnum.Massage2;

                    case ProgramMode.pilates:
                        return RoomEnum.Personal2;

                    case ProgramMode.pilatesFunctional:
                        if (ProgramMode == ProgramMode.pilates)
                            return RoomEnum.Personal2;
                        else
                            return RoomEnum.Fitness;

                    case ProgramMode.personal:
                        return RoomEnum.FreeSpace;
                }
                return RoomEnum.Fitness;
            }

            switch (ProgramModeNew)
            {
                case ProgramMode.functional:
                    return RoomEnum.Functional;

                case ProgramMode.massage:
                    return RoomEnum.Massage;

                case ProgramMode.online:
                case ProgramMode.outdoor:
                case ProgramMode.aerialYoga:
                case ProgramMode.medical:
                    return RoomEnum.Outdoor;

                case ProgramMode.pilates:
                    return RoomEnum.Pilates;

                case ProgramMode.yoga:
                    return RoomEnum.Outdoor;

                case ProgramMode.pilatesFunctional:
                    if (ProgramMode == ProgramMode.pilates)
                        return RoomEnum.Pilates;
                    else
                        return RoomEnum.Functional;

                case ProgramMode.personal:
                    return RoomEnum.Personal;
            }
            return RoomEnum.Functional;
        }

        private string GetDescription()
        {
            var res = string.Empty;
            res += GetProogramName();
            if (Is30min)
            {
                res += ", 30'";
            }

            if (Present)
            {
                res += ", Δώρο";
            }

            if (!Real)
            {
                res += ", Δεν ήρθε";
            }

            return res;
        }

        private string GetProogramName()
        {
            if (Prog != null && Prog.ProgramTypeO != null)
            {
                return Prog.ProgramTypeO.ProgramName;
            }
            switch (ProgramModeNew)
            {
                case ProgramMode.functional:
                    return "Γυμναστική";

                case ProgramMode.massage:
                    return "Massage";

                case ProgramMode.online:
                    return "Online";

                case ProgramMode.outdoor:
                    return "Outdoor";

                case ProgramMode.pilates:
                    return "Pilates";

                case ProgramMode.yoga:
                    return "Yoga";

                case ProgramMode.pilatesFunctional:
                    return "Pil+Fun";

                case ProgramMode.aerialYoga:
                    return "Aerial";

                case ProgramMode.personal:
                    return "Personal";

                case ProgramMode.medical:
                    return "Medical";

                default:
                    return "Σφάλμα";
            }
        }

        public DateTime Arrived
        {
            get => _Time;

            set
            {
                if (_Time == value)
                {
                    return;
                }
                if (value == null)
                {
                    RaisePropertyChanged();
                    return;
                }

                _Time = value;
                RaisePropertyChanged();
            }
        }

        [NotMapped]
        public SolidColorBrush Color
        {
            get => _Color;

            set
            {
                if (_Color == value)
                {
                    return;
                }

                _Color = value;
                RaisePropertyChanged();
            }
        }

        public Customer Customer { get; set; }

        public DateTime Left
        {
            get => _Left;

            set
            {
                if (_Left == value)
                {
                    return;
                }

                _Left = value;
                RaisePropertyChanged();
            }
        }

        public int? Prog_Id { get; set; }

        #endregion Properties
    }
}