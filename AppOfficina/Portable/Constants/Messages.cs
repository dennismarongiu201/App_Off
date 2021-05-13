using System;
using System.Collections.Generic;
using System.Text;

namespace AppOfficina.Constants
{
    public static class Messages
    {
        public const string GlobalSuccessMsg = "Operazione completata con successo!";
        public const string UnhandledExceptionError = "Errore dell'applicazione. Contattare l'amministratore di sistema";
        public const string NoInternetConnection = "Per questa operazione è richiesta una connessione ad internet";
        public const string UnAuthorizedLoginAgain = "Non sei autorizzato, effettua nuovamente l'accesso!";
        
        public static class Login
        {
            public const string IncorrectPassword = "Password errata.";
            public const string UnknownApiCredentialsCheck = "Incontrato un errore sconosciuto al controllo delle credenziali. Contatta l'amministratore di sistema.";
            public const string InvalidCredentials = "Le credenziali sono invalide , controllare se sono stati inseriti correttamente i campi.";
            public const string ApiUserNotFound = "Nome utente o password errate.";
            public const string UnhandledExceptionError = "Si è verificato un errore nell'autenticazione dell'utente, Contattare l'amministratore di sistema.";
        }

        public static class Commessa
        {
            public const string NoFoundCommessa = "Nessuna commessa trovata.";
            public const string NoFoundCommessaForTarga = "Nessuna commessa trovata per la targa inserita.";
            public const string UnknownApiCommessaCheck = "Incontrato un errore sconosciuto alla ricerca delle commesse. Contatta l'amministratore di sistema.";
            public const string InvalidFieldsCommessaTarga = "Per cercare la commessa occorre inserire il numero o la targa.";
            public const string UnhandledExceptionError = "Si è verificato un errore nel servizio della ricerca commessa , Contattare l'amministratore di sistema.";
        }

        public static class Inconveniente
        {
            public const string InsertSuccessInconveniente = "Inconveniente inserito correttamente";
            public const string NoFoundInconveniente = "Nessun inconveniente per questa commessa!!";
            public const string UnknownApiInconvenienteCheck = "Incontrato un errore sconosciuto all'inserimento dell'inconveniente . Contatta l'amministratore di sistema";
            public const string InvalidFieldsInconvenienteExtra = "Per inserire l'inconveniente extra il titolo e la descrizione sono obbligatori! Inserire tutti i dati richiesti.";
            public const string InsertNote = "Nota aggiornata correttamente";

            public const string StatusInconvenienteRicambi = "Attenzione! l'inconveniente deve essere in corso per poterlo mettere in attesa dei ricambi !";
            public const string UnhandledExceptionError = "Si è verificato un errore nella chiamata al servizio dell'inconveniente , Contattare l'amministratore di sistema.";
        }

        public static class Camera
        {
            public const string NoCameraAvaible = "La camera non é disponibile per questo dispositivo";
        }


    }
}
