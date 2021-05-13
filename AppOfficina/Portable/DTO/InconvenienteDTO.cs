using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Input;

namespace AppOfficina.Portable.DTO
{
    public class InconvenienteDTO : INotifyPropertyChanged
    {
        public string NomeInconveniente { get; set; }
        public string Stato { get; set; }

        public string NumeroInconveniente { get; set; }
        public bool Extra { get; set; }
        public Color ColorState { get; set; }

        public string Descrizione { get; set; }
        public int IdMarcatempo { get; set; }

        public int NumeroFoto { get; set; }
        public int NumeroDocumenti { get; set; }

        private DateTime _DataInizioLavorazione;
        public DateTime DataInizioLavorazione
        {
            get
            {
                return _DataInizioLavorazione;
            }
            set
            {
                _DataInizioLavorazione = value;
                OnPropertyChanged("DataInizioLavorazione");
            }

        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _OreLavorate;
        public int OreLavorate
        {
            get
            {
                return _OreLavorate;
            }
            set
            {
                _OreLavorate = value;
                OnPropertyChanged("OreLavorate");
            }
        }

        private double _MinutiLavorati;
        public double MinutiLavorati
        {
            get
            {
                return _MinutiLavorati;
            }
            set
            {
                _MinutiLavorati = value;
                OnPropertyChanged("MinutiLavorati");
            }
        }


        private string _NoteInconveniente;
        public string NoteInconveniente
        {
            get
            {
                return _NoteInconveniente;
            }
            set
            {
                _NoteInconveniente = value;
                OnPropertyChanged("NoteInconveniente");

            }
        }





        private string _DataInizio;
        public string DataInizio
        {
            get
            {
                if (_DataInizio == null)
                {
                    _DataInizio = string.Empty;
                }
                return _DataInizio;
            }
            set
            {
                _DataInizio = value;
                OnPropertyChanged("DataInizio");
            }
        }

        private string _DataFine;
        public string DataFine
        {
            get
            {
                if (_DataFine == null)
                {
                    _DataFine = string.Empty;
                }
                return _DataFine;
            }
            set
            {
                _DataFine = value;
                OnPropertyChanged("DataFine");
            }
        }


        private double _SecondiLavorati;
        public double SecondiLavorati
        {
            get
            {
                return _SecondiLavorati;
            }
            set
            {
                _SecondiLavorati = value;
                OnPropertyChanged("SecondiLavorati");
            }
        }


        private string _Note;
        public string Note
        {
            get
            {
                return _Note;
            }
            set
            {
                _Note = value;
                OnPropertyChanged("Note");

            }
        }



        private bool _visibleDateInizioLavorazione;
        public bool visibleDateInizioLavorazione
        {
            get
            {
                return _visibleDateInizioLavorazione;
            }
            set
            {
                _visibleDateInizioLavorazione = value;
                OnPropertyChanged("visibleDateInizioLavorazione");

            }
        }



        private bool _IsEnableNote;
        public bool IsEnableNote
        {
            get
            {
                return _IsEnableNote;
            }
            set
            {
                _IsEnableNote = value;
                OnPropertyChanged("IsEnableNote");

            }
        }

        private string _nameImage;
        public string nameImage
        {
            get
            {
                return _nameImage;
            }
            set
            {
                _nameImage = value;
                OnPropertyChanged("nameImage");
            }
        }



        private string _timerInEsecuzione;
        public string timerInEsecuzione
        {
            get
            {
                return _timerInEsecuzione;
            }
            set
            {
                _timerInEsecuzione = value;
                OnPropertyChanged("timerInEsecuzione");

            }
        }


        private string _StateLavoro;
        public string StateLavoro
        {
            get
            {
                return _StateLavoro;
            }
            set
            {
                _StateLavoro = value;
                OnPropertyChanged("StateLavoro");

            }
        }

        private bool _IsLoadTimer;
        public bool IsLoadTimer
        {
            get
            {
                return _IsLoadTimer;
            }
            set
            {
                _IsLoadTimer = value;
                OnPropertyChanged("IsLoadTimer");

            }
        }

        private bool _selection;
        public bool selection
        {
            get
            {
                return _selection;
            }
            set
            {
                _selection = value;
                OnPropertyChanged("selection");

            }
        }

        //private bool _isEnable;
        //public bool isEnable
        //{
        //    get
        //    {
        //        return _isEnable;
        //    }
        //    set
        //    {
        //        _isEnable = value;
        //        OnPropertyChanged("isEnable");

        //    }
        //}
    }

    public class InconvenienteRequest
    {
        public string NumeroCommessa { get; set; }
    }

    public class InconvenienteNoteDTO
    {
        public string NumeroInconveniente { get; set; }
        public bool Extra { get; set; }
        public string Note { get; set; }
    }

    public class NoteRequestDTO
    {
        public string NumeroInconveniente { get; set; }
    }

    public class RequestInsertNotaDTO
    {
        public string NumeroInconveniente { get; set; }
        public string TestoNota { get; set; }
    }


    public class InconvenienteWSDTO
    {
        public string NumeroInconveniente { get; set; }
        public bool Extra { get; set; }
        public DateTime DataInizioLavoro { get; set; }
    }

    public class InconvenienteInvioTempoDTO
    {
        public string NumeroInconveniente { get; set; }
        public bool Extra { get; set; }

        public DateTime DataInizioLavoro { get; set; }

        public DateTime DataFineLavoro { get; set; }
        public string Note { get; set; }

        public int IdMarcatempo { get; set; }
    }

}