// SEARCH =ANSLIBF, =RMS_LIB_CKRALG03
// SEARCH =RMS_LIB_CKR3007C,=RMS_LIB_CKR3010C,=RMS_LIB_CKR3016C
// SEARCH =RMS_LIB_CKR3031C,=RMS_LIB_CKR3032C
// SEARCH =RMS_LIB_CKR3039C
// SEARCH =RMS_LIB_T270001I
namespace AVT
{
    public class CN27001C
    {
        // *****************************************************************************
        //  avt-app
        //   Proces 270      Raadplegen AVT
        //   FILE            $DATA3.CKRCOBT.CN27001C
        //                   Dit is de CKR-Net-versie !!!
        //   APPLICATIE      CKR - Centrale Klanten Registratie PTT Telecom
        //   RELEASE         25.2
        //   DATUM AANMAAK   08-03-2005
        //   BESCHRIJVING    Maakt raadplegen AVT-meldingen mogelijk
        //   AUTEUR          Ruben Deurloo
        //   INPUT           via $RECEIVE  CKR270001I
        //   OUTPUT          via $RECEIVE  CKR270001U
        //   LOGGING         file "LOG"
        //   GEBRUIKT
        // 
        //   VERIFICATIE     wv-603
        // 
        //   AANGEBRACHTE WIJZIGINGEN
        //   ---------+------------+-----------------------------------------------------
        //   NAAM     | DATUM      | OMSCHRIJVING
        //   ---------+------------+-----------------------------------------------------
        //   JaTo     | 13-10-2005 | Rel 27.0 FWD635 Toevoeg Controle admin-grp/code-appl
        //   ---------+------------+-----------------------------------------------------
        //  ENVIRONMENT DIVISION.
        //  CONFIGURATION SECTION.
        //  SOURCE-COMPUTER.  T16.
        //  OBJECT-COMPUTER.  T16.
        //  SPECIAL-NAMES.
        //      DECIMAL-POINT IS COMMA.
        //  INPUT-OUTPUT SECTION.
        //  FILE-CONTROL.
        // ---------------------------------------------------- D A T A   D I V I S I O N
        //  DATA DIVISION.
        //  FILE SECTION.
        // ********************************************* W O R K I N G - S T O R A G E
        //  WORKING-STORAGE SECTION.
        //  In- en uitvoerrecords
        //  COPY CKR270001U  OF "=RMS_CPC_NETSRC".
        //  COPY WS-ONLINE-I OF "=RMS_CPC_NETSRC".
        //  COPY WS-ONLINE-U OF "=RMS_CPC_NETSRC".
        //  COPY COD-HER-STAT-TRANS-WAARDES OF "=RMS_CPC_COBSRC".
        //    Response buffer
        //  COPY ANTWOORDEN270-STRK OF "=RMS_CPC_COBSRC" REPLACING ANTWOORDEN270-STRK
        //         BY CKRRESPBUF.
        //    Aantal response buffers
        //  01 VOLGNR-RESP-TELLER                          PIC 9(4).
        //    Aantal teruggegeven antwoorden
        //  01 ANTWOORD-TELLER                             PIC 9(8) VALUE 0.
        //    Aantal antwoorden teruggegeven via CKR270001U
        //  01 AANT-ONLINE-GEVONDEN                        PIC 9(4) COMP VALUE 0.
        //    Aantal regels in de response buffer
        //  01 RESPONSE-REGELS                             PIC 9(2) VALUE 0.
        //  Eigen werkvelden.
        //  Indicator zoekpad
        //  01 INDICATOR-ZOEKPAD                           PIC X(01).
        //     88 ZOEKPAD-CKR-NR                           VALUE "C".
        //     88 ZOEKPAD-TELECOM-NR                       VALUE "T".
        //  Indicatoren cursoren
        //  01 INDICATOR-CUR01-AVT-VERMELD                 PIC X(01).
        //     88 CUR01-AVT-VERMELD-OPEN                   VALUE "J".
        //     88 CUR01-AVT-VERMELD-GESLOTEN               VALUE "N".
        //  01 INDICATOR-CUR02-AVT-VERMELD                 PIC X(01).
        //     88 CUR02-AVT-VERMELD-OPEN                   VALUE "J".
        //     88 CUR02-AVT-VERMELD-GESLOTEN               VALUE "N".
        //  01 STATUS-EOF-CUR01                            PIC X(01).
        //     88 NOT-EOF-CUR01                            VALUE "N".
        //     88 EOF-CUR01                                VALUE "J".
        //  01 STATUS-EOF-CUR02                            PIC X(01).
        //     88 NOT-EOF-CUR02                            VALUE "N".
        //     88 EOF-CUR02                                VALUE "J".
        //  01 CKR-NR-OMBUIG-STATUS                        PIC X(01).
        //     88 CKR-NR-IN-OMBUIG                         VALUE "J".
        //     88 CKR-NR-NIET-IN-OMBUIG                    VALUE "N".
        //  01 STATUS-GEHEIME-NRS-GEVONDEN                 PIC X(01).
        //     88 GEEN-GEHEIME-NRS-GEVONDEN                VALUE "N".
        //     88 GEHEIME-NRS-GEVONDEN                     VALUE "J".
        //  Diverse constanten
        //  01 LENGTE-INFALG-270                           PIC 9(04) VALUE 16.
        //  01 LENGTE-HEADER-270                           PIC 9(04) VALUE 109.
        //  01 LENGTE-HEADER-ANS                           PIC 9(04) VALUE 20.
        //  01 AANTAL-OCCURS-270                           PIC 9(04) VALUE 59.
        //  01 LENGTE-1-ANTWOORD-270                       PIC 9(04) VALUE 51.
        //    Lengte van de hele antwoord buffer
        //  01 LENGTE-DATA                                 PIC 9(04) COMP VALUE 0.
        //   Hulpvelden t.b.v ANS_RECEIVE en ANS_REPLY
        //  01 STARTUP-MESSAGE                       PIC X(596).
        //  77 APPLICATIENAAM                        PIC X(06) VALUE "CKR".
        //  77 ANS-INIT-STATUS                       PIC S9(04) COMP.
        //  77 ANS-RCV-STATUS                        PIC S9(04) COMP.
        //  77 ANS-RPL-STATUS                        PIC S9(04) COMP.
        //  77 BERICHTLENGTE                         PIC S9(04) COMP.
        //  77 LARGE-BUFFER                          PIC X(3200).
        //    Hulpvelden en statussen worden als 03-level gedefinieerd om te
        //    voorkomen dat ze allemaal word-aligned worden.
        //  01 HULPVELDEN.
        //  Als "NIET-DOORGAAN" dan stopt het hele programma.
        //     03 DOORGAAN-STATUS                          PIC X(01).
        //        88 DOORGAAN                              VALUE "J".
        //        88 NIET-DOORGAAN                         VALUE "N".
        //  Als "VERW-NIET-OKEE" dan kan melding niet getoond en
        //  gaat er een foutmelding naar de applicatie.
        //     03 VERW-STATUS                              PIC X(01).
        //        88 VERW-OKEE                             VALUE "J".
        //        88 VERW-NIET-OKEE                        VALUE "N".
        //  Wordt geset in de X900-SQL-NOTFOUND.
        //     03 SQL-FOUND-STATUS                         PIC X(01).
        //        88 SQL-FOUND                             VALUE "J".
        //        88 SQL-NOTFOUND                          VALUE "N".
        //  Wordt geset in de X910-SQLERROR (en evt. in X900-NOTFOUND)
        //     03 SQL-STATUS                               PIC X(01).
        //        88 SQL-OKEE                              VALUE "J".
        //        88 SQL-FOUT                              VALUE "N".
        //  Als een NOTFOUND optreedt, moet bekend zijn of dit duidt op een
        //  integriteitsfout in de database. Deze moeten worden gelogd.
        //     03 LOG-WHEN-NOTFOUND-STATUS                 PIC X(01).
        //        88 LOG-WHEN-NOTFOUND                     VALUE "J".
        //  Hulpveld voor het afvragen van de afkomst (batch/online) van de transactie
        //     03 WS-COD-HER                               PIC X(01).
        //        88 HER-ONLINE                            VALUE "O".
        //        88 HER-BATCH                             VALUE "B".
        //  Hulpveld tbv het maken van de foutboodschap
        //     03 HULP-VERSIE                              PIC X(03).
        //     03 HULP-TEKST                               PIC X(73).
        //     03 HULP-TEKST-2                             PIC X(11).
        //  01 CKR-NET-HULPVELDEN.
        //     03 WS-MESSAGE                               PIC X(80).
        //     03 BUFFER-NAAM.
        //        05 DEEL-1                                PIC X(08).
        //        05 DEEL-2                                PIC X(06).
        //     03 WS-COD-TRANS                             PIC X(03).
        //     03 WS-COD-TRANS-VERSIE                      PIC X(03).
        //     03 WS-NM-RESP-BUF.
        //        05 WS-DAT-RESP                           PIC X(08).
        //        05 WS-RESP-BUF-NR                        PIC X(06).
        //  COPY CKR3007U      OF "=RMS_CPC_COBSRC".
        //  COPY CKR3010I      OF "=RMS_CPC_COBSRC".
        //  COPY CKR3016I      OF "=RMS_CPC_COBSRC".
        //  COPY CKR3016U      OF "=RMS_CPC_COBSRC".
        //  COPY CKR3020I      OF "=RMS_CPC_COBSRC".
        //  COPY CKR3032I      OF "=RMS_CPC_COBSRC".
        //  COPY CKR3032U      OF "=RMS_CPC_COBSRC".
        //  COPY CKR3039I      OF "=RMS_CPC_COBSRC".
        // ********************************************** S Q L   D E C L A R A T I E S
        //  EXEC SQL INCLUDE SQLCA                         END-EXEC.
        // 
        //  EXEC SQL  BEGIN DECLARE SECTION                END-EXEC.
        // 
        //   "BEGIN DECLARE SECTION" duidt aan dat de SQL-precompiler de volgende velden
        //   (tot aan de END DECLARE SECTION) moet herkennen in SQL-commando's.
        // 
        // EXEC SQL CONTROL TABLE =AVT_VERMELD AS AV ACCESS PATH INDEX =AVT_IVERML02
        //                                                                  END-EXEC.
        //  EXEC SQL       INVOKE  =AVT_VERMELD AS WS-AVT-VERMELD     END-EXEC.
        //  EXEC SQL       INVOKE  =RESPBUF     AS WS-RESPBUF         END-EXEC.
        //  EXEC SQL       INVOKE  =VESTIG      AS WS-VESTIG          END-EXEC.
        //  EXEC SQL       INVOKE  =OMBUIG      AS WS-OMBUIG          END-EXEC.
        //  Control Table statements inbouwen i.v.m. tablelocks bij grote transacties
        //  EXEC SQL       CONTROL TABLE =AVT_VERMELD  TABLELOCK OFF    END-EXEC.
        //  EXEC SQL       CONTROL TABLE =VESTIG       TABLELOCK OFF    END-EXEC.
        // 
        // SOURCE =RMS_CPC_NETSRC ( CKR270001I )
        // 
        //    Huidige systeemdatum.
        //  01 DATUM-VANDAAG                               PIC 9(08) COMP.
        //  01 SYSTEEM-TIJD                                PIC 9(08) COMP.
        //  01 WS-DUMMY-CKRNR                              PIC X(08).
        // 
        //  Hulpvelden om alfanumerieke velden in een tabel in te voegen
        //  waar numerieke velden worden verwacht
        //  Deze waarden worden in D100 gevuld en kunnen vervolgens in de rest van het
        //  programma worden gebruikt.
        //  01 SQL-HULPVELDEN.
        //     03 WS-COD-TRANS                              PIC 9(03).
        //     03 WS-COD-APPL                               PIC 9(03).
        //  Alle hulpvelden indien mogelijk copieren uit de DDL.
        //  COPY CKR-NR                     OF "=RMS_CPC_COBSRC"
        //     REPLACING CKR-NR BY WS-CKR-NR.
        //  EXEC SQL END DECLARE SECTION                    END-EXEC.
        // ********************************** C U R S O R   D E F I N I T I E********
        //  EXEC SQL DECLARE CUR01_AVT_VERMELD CURSOR FOR
        //          SELECT  ABONNEE_GEG_NR
        //                , ADMIN_GRP
        //                , TELECOM_NR
        //                , AV.CKR_NR
        //                , B_NR_AFSCHERMING
        //                , PUBLICATIE_CODE
        //                , NETWERK_TYPE
        //                , GEBRUIK_NETLIJN
        //                , DATUM_INGANG
        //                , DATUM_INVOER
        //                , TIJD_INVOER
        //                , COD_APPL_INVOER
        //                , USERID_INVOER FROM   =AVT_VERMELD AV
        //          WHERE AV.CKR_NR = :WS-CKR-NR AND AV.ADMIN_GRP IN (SELECT AA.ADMIN_GRP FROM =AVT_ADMINAPP AA
        //                                  WHERE AA.COD_APPL = :WS-COD-APPL
        //                                    FOR BROWSE ACCESS)
        //          BROWSE ACCESS
        //  END-EXEC
        //  EXEC SQL DECLARE CUR02_AVT_VERMELD CURSOR FOR
        //          SELECT  ABONNEE_GEG_NR
        //                , ADMIN_GRP
        //                , TELECOM_NR
        //                , AV.CKR_NR
        //                , B_NR_AFSCHERMING
        //                , PUBLICATIE_CODE
        //                , NETWERK_TYPE
        //                , GEBRUIK_NETLIJN
        //                , DATUM_INGANG
        //                , DATUM_INVOER
        //                , TIJD_INVOER
        //                , COD_APPL_INVOER
        //                , USERID_INVOER FROM   =AVT_VERMELD AV,
        //                 =OMBUIG O
        //          WHERE O.CKR_NR = :WS-CKR-NR AND O.CKR_NR_OUD = AV.CKR_NR AND AV.ADMIN_GRP IN (SELECT AA.ADMIN_GRP FROM =AVT_ADMINAPP AA
        //                                  WHERE AA.COD_APPL = :WS-COD-APPL
        //                                    FOR BROWSE ACCESS)
        //          BROWSE ACCESS
        //  END-EXEC
        // *************************  E X T E N D E D - S T O R A G E   S E C T I O N **
        //  Deze extended-storage section is toegevoegd omdat de normale working-storage
        //  niet alle variabelen "aankon"
        // ********************************** P R O C E D U R E    D I V I S I O N***
        //  PROCEDURE DIVISION.
        public void WheneverSection()
        {
            //  EXEC SQL WHENEVER NOTFOUND   PERFORM :X900-NOTFOUND   END-EXEC.
            //  EXEC SQL WHENEVER SQLERROR   PERFORM :X910-SQLERROR   END-EXEC.
            //  EXEC SQL WHENEVER SQLWARNING PERFORM :X920-SQLWARNING END-EXEC.
        }

        public void A000_MAIN()
        {
        A000_010:
            B100_INITIALISATIE();
            B200_CENTRALE_VERWERKING();
            //=        UNTIL NIET-DOORGAAN
            B300_AFSLUITING();

        A000_999:
            return;
            // ----------------------------------------------------------------------------
        }

        public void B100_INITIALISATIE()
        {
        B100_010:
            //=      INITIALIZE CKR3010I
            DOORGAAN_STATUS = "J";

        B100_999:
            return;
            // ----------------------------------------------------------------------------
        }

        public void B200_CENTRALE_VERWERKING()
        {
        B200_010:
            C100_LEES_REQUEST();
            if (NIET_DOORGAAN)
            {
                //=         GO TO B200-999
            }
            C200_INIT_REQUEST();
            if (VERW_OKEE)
            {
                C300_VERWERK_REQUEST();
            }
            C400_AFSLUIT_REQUEST();
            C500_SCHRIJF_REPLY();

        B200_999:
            return;
            // ---------------------------------------------------------------------------
        }

        public void B300_AFSLUITING()
        {
        B300_010:
        //=      CONTINUE.
        B300_999:
            return;
            // -----------------------------------------------------------------------------
        }

        public void C100_LEES_REQUEST()
{
C100_010:
//=      INITIALIZE CKR3010I
//=      INITIALIZE CKR3039I
ANS_RCV_STATUS = Process.EnterTal("ANS_RECEIVE", APPLICATIENAAM, LARGE_BUFFER, BERICHTLENGTE);
//     ANS-receive geeft status 2 terug als een STOP-message werd ontvangen.
if ( ( ANS_RCV_STATUS == 2))
{
DOORGAAN_STATUS = "N";
//=         GO TO C100-999
}
if ( ( ANS_RCV_STATUS == 1))
{
CKR3010I.CKR_FOUT = 6;
CKR3010I.TEKST = "READ-ERROR OP $RECEIVE";
X666_ZEND_LOG();
DOORGAAN_STATUS = "N";
}
//  Bepalen van de begintijd van de transactie.
//=      ACCEPT TIJD-AANROEP  OF CKR3039I FROM TIME
WS_ONLINE_I = LARGE_BUFFER;
//  Afvragen herkomst
if ( WS_ONLINE_I.ANS_RESTANT IS == TO SPACES)
{
WS_COD_HER = "B";
} else {
WS_COD_HER = "O";
}

C100_999:
return;
// ------------------------------------------------------------------------------
}

        public void C200_INIT_REQUEST()
        {
        C200_010:
            //=      ACCEPT DATUM-VANDAAG FROM DATE
            //=      COMPUTE DATUM-VANDAAG = DATUM-VANDAAG + 20000000 END-COMPUTE
            //=      ACCEPT SYSTEEM-TIJD FROM TIME
            //=      INITIALIZE CKRRESPBUF
            //=      INITIALIZE WS-AVT-VERMELD
            //=      INITIALIZE CKR270001U
            //=      INITIALIZE WS-ONLINE-U
            WS_ONLINE_U.STATRA = ZERO;
            WS_ONLINE_U.VERREC = "N";
            VERW_STATUS = "J";
            SQL_FOUND_STATUS = "J";
            SQL_STATUS = "J";
            STATUS_GEHEIME_NRS_GEVONDEN = "N";
            WS_CKR_NR = SPACE;
            AANT_ONLINE_GEVONDEN = 0;
            RESPONSE_REGELS = 0;
            VOLGNR_RESP_TELLER = 1;
            INDICATOR_CUR01_AVT_VERMELD = "N";
            INDICATOR_CUR02_AVT_VERMELD = "N";
            STATUS_EOF_CUR01 = "N";
            STATUS_EOF_CUR02 = "N";
            VOLGNR_RESP_TELLER = 1;
            //     Begin van controles
            //     Rubriekscontrole
            if (VERW_OKEE)
            {
                D100_RUBRIEKSCONTROLE();
            }
            //  Vestigingscontrole
            if (VERW_OKEE)
            {
                if (NOT(CKR270001I.CKR_NR == SPACES))
                {
                    D125_VESTIGINGSCONTROLE();
                }
            }
            //  Is gebruiker geautoriseerd om geheime nummers te bekijken?
            if (VERW_OKEE)
            {
                D150_AUTORISATIE_CONTROLE();
            }
            //  Haal eerste record
            if (VERW_OKEE)
            {
                D200_HAAL_VERMELD();
            }

        C200_999:
            return;
            // -----------------------------------------------------------------------------
        }

        public void C300_VERWERK_REQUEST()
{
C300_010:
//  Request verwerken.
if ( CKR270001I.CKR_NR == SPACES)
{
//  Er is gezocht op TELECOM-NR, derhalve slechts ��n resultaat mogelijk.
ANTWOORD_TELLER = 1;
CKR270001U.AANT_GEVONDEN_RECORDS = 1;
AANT_ONLINE_GEVONDEN = 1;
WS_CKR_NR = WS_AVT_VERMELD.CKR_NR;
//  Kijken of CKR-nr bestaat
LOG_WHEN_NOTFOUND_STATUS = "N";
SQL04_S_VESTIG();
if ( SQL_FOUT)
{
//=            GO TO C300-999
}
if ( SQL_NOTFOUND)
{
//  Er moet een ombuiging bestaan
LOG_WHEN_NOTFOUND_STATUS = "J";
SQL05_S_OMBUIG();
if ( SQL_FOUT)
{
//=               GO TO C300-999
}
//  Als gevonden CKR-NR omgebogen is tonen we het nieuwe CKR-NR.....
//  ... zonder waarschuwing dat er omgebogen is.
//  Eerst nog even onderzoeken of de terug te geven vestiging bestaat.
WS_CKR_NR = WS_OMBUIG.CKR_NR;
LOG_WHEN_NOTFOUND_STATUS = "J";
SQL04_S_VESTIG();
if ( SQL_FOUT)
{
//=               GO TO C300-999
}
}
if ( WS_AVT_VERMELD.Substring(4,1).PUBLICATIE_CODE == "J")
{
WS_ONLINE_U.STATRA = "0131";
WS_ONLINE_U.HERSTA = MELDING_CKRC;
}
D400_VULLEN_CKR270001U();
} else {
//  Er wordt gezocht op CKR-nr, meerdere hits mogelijk
ANTWOORD_TELLER = 0;
//=         PERFORM UNTIL (EOF-CUR02)   OR (VERW-NIET-OKEE)
//  Betreft het een geheim nummer en een niet geautoriseerde gebruiker dan
//  wordt er niet geschreven.
if ( WS_AVT_VERMELD.Substring(4,1).PUBLICATIE_CODE == "J" AND)
{
//=               IND-TOEGESTAAN OF CKR3016U = "N"
WS_ONLINE_U.STATRA = "0132";
WS_ONLINE_U.HERSTA = MELDING_CKRC;
STATUS_GEHEIME_NRS_GEVONDEN = "J";
} else {
//=               ADD 1 TO ANTWOORD-TELLER
//  Eerst vullen we CKR27001U......
if ( ANTWOORD_TELLER <= AANTAL_OCCURS_270)
{
AANT_ONLINE_GEVONDEN = ANTWOORD_TELLER;
D400_VULLEN_CKR270001U();
} else {
//  .... en als die vol is de response buffer
//=                  ADD 1 TO RESPONSE-REGELS
D500_VULLEN_RESPBUF();
if ( VERW_NIET_OKEE)
{
//=                     GO TO C300-999
}
}
}
//      -- Haal nieuwe VERMELD
D300_HAAL_VOLGENDE();
//=         END-PERFORM
//  Als er alleen maar geheime nummers gevonden zijn, geef dan foutmelding
if ( ANTWOORD_TELLER == 0 && GEHEIME_NRS_GEVONDEN)
{
VERW_STATUS = "N";
WS_ONLINE_U.STATRA = "0260";
WS_ONLINE_U.HERSTA = DBFOUT_CKRC;
//=            GO TO C300-999
}
if ( RESPONSE_REGELS > 0)
{
WS_RESPBUF.DAT_RESP = DATUM_VANDAAG;
WS_RESPBUF.RESP_BUF_NR = CKR3007U.RESP_BUF_NR_NW;
WS_RESPBUF.VOLGNR_RESP = VOLGNR_RESP_TELLER;
//=            COMPUTE LENGTE-DATA = LENGTE-1-ANTWOORD-270 * RESPONSE-REGELS
WS_RESPBUF.LNGTE_RESP_BUF = LENGTE_DATA;
WS_RESPBUF.RESP_BUF_DATA = CKRRESPBUF;
SQL16_I_RESPBUF();
if ( SQL_FOUT)
{
//=               GO TO C300-999
}
}
if ( VERW_OKEE)
{
CKR270001U.AANT_GEVONDEN_RECORDS = ANTWOORD_TELLER;
}
}

C300_999:
return;
// -----------------------------------------------------------------------------
}

        public void C400_AFSLUIT_REQUEST()
{
C400_010:
//     Wanneer tijdens verwerking een fatale error optrad en dus X666-ZEND-LOG
//     gebruikt is, wordt dit aan de gebruiker gemeld door een STAT-TRANS van
//     97. De responder moet dit ook weten en die let op de STAT-IPM.
if ( WS_ONLINE_U.STATRA == "0097")
{
WS_ONLINE_U.STAT_IPM = 999;
}
//     Vullen van een eventuele foutmelding
if ( ( WS_ONLINE_U.HERSTA != SPACE && "A" && "M" ) AND)
{
//=           INDMSG OF WS-ONLINE-I NOT = "N"
N300_VUL_FOUTMELDING();
}
WS_ONLINE_U.INFTXT = CKR270001U.INFTXT;

C400_999:
return;
// -----------------------------------------------------------------------------
}

        public void C500_SCHRIJF_REPLY()
{
C500_010:
//  Als het request van een Screencobol requester afkomstig is( testrequesters)
//  moet de lengte van de IPM een vaste waarde hebben, nl. 3200
if ( WS_ONLINE_I.CODSYS == "*TEST*")
{
BERICHTLENGTE = 3200;
} else {
//  Bij fouten moet alleen de header, dus voor INFTXT worden doorgegeven !
//  In beide gevallen moet er 20 bij opgeteld worden omdat in WS-ONLINE-U ook de
//  ANS-HEADER moet worden doorgegeven
if ( WS_ONLINE_U.HERSTA != SPACE AND)
{
//=            HERSTA OF WS-ONLINE-U NOT = "W"   AND
//=            HERSTA OF WS-ONLINE-U NOT = "X"
//=            COMPUTE BERICHTLENGTE =
//=               LENGTE-HEADER-ANS + LENGTE-HEADER-270
//=            END-COMPUTE
} else {
//=            COMPUTE BERICHTLENGTE =
//=               LENGTE-HEADER-ANS + LENGTE-HEADER-270 + LENGTE-INFALG-270
//=               + (AANT-ONLINE-GEVONDEN * LENGTE-1-ANTWOORD-270)
//=            END-COMPUTE
}
}
//   De "ANS-HEADER" moet onveranderd worden overgezet. Doe je dit niet, dan
//   blijft de "ANS-HEADER" in ws-online-u leeg en komt de boodschap niet
//   over. Uiteindelijk resulteert dit in een time-out (ans-status 2).
WS_ONLINE_U.STAT_IPM = WS_ONLINE_I.STAT_IPM;
WS_ONLINE_U.ANS_RESTANT = WS_ONLINE_I.ANS_RESTANT;
ANS_RPL_STATUS = Process.EnterTal("ANS_REPLY", APPLICATIENAAM, WS_ONLINE_U, BERICHTLENGTE);
if ( ( ANS_RPL_STATUS != ZERO))
{
CKR3010I.CKR_FOUT = 6;
CKR3010I.TEKST = "WRITE-ERROR OP $RECEIVE";
X666_ZEND_LOG();
DOORGAAN_STATUS = "N";
}
//=      ACCEPT TIJD-ANTWOORD OF CKR3039I FROM TIME
//  Loggen van de gegevens
X777_LOG_TRANSACTIE();

C500_999:
return;
// -----------------------------------------------------------------------------
}

        public void D100_RUBRIEKSCONTROLE()
{
D100_010:
N100_CONTROLEER_HEADER();
if ( VERW_OKEE)
{
N200_CONTROLEER_TRANSACTIE();
}
CKR270001I = WS_ONLINE_I.CKR_DEEL;
//  In het invoer-record zijn alle velden alfa-numeriek. Hier wordt een aantal
//  hulpvelden gevuld, die numeriek moeten zijn
SQL_HULPVELDEN.WS_COD_APPL = CKR270001I.COD_APPL;
SQL_HULPVELDEN.WS_COD_TRANS = CKR270001I.COD_TRANS;
//  Als er tijdens de controles van CKR-Net een fout is geconstateerd, dan wordt
//  de VERW-STATUS hier op "N" gezet.
if ( WS_ONLINE_U.HERSTA != SPACE)
{
VERW_STATUS = "N";
//=         GO TO D100-999
}
//  Autorisatiecontrole (D 0125)
CKR3032I.COD_TRANS = WS_ONLINE_I.CODTRA;
CKR3032I.COD_TRANSVERSIE = WS_ONLINE_I.CODTRAVER;
CKR3032I.COD_APPL = WS_ONLINE_I.CODAPL;
CKR3032I.IND_BATCH_ONLINE = WS_COD_HER;
Process.Call("CKR3032C", CKR3032I);
//=                             CKR3032U, CKR3010I
if ( CKR3010I.CKR_FOUT != ZERO)
{
CKR3010I.TEKST = SPACES;
CKR3010I.CKR_FOUT = 7;
CKR3010I.TEKST = string.Format("FOUT IN MODULE 3032", );
X666_ZEND_LOG();
VERW_STATUS = "N";
WS_ONLINE_U.STATRA = "0097";
WS_ONLINE_U.HERSTA = HARDE_DBFOUT;
//=         GO TO D100-999
}
if ( CKR3032U.IND_TOEGESTAAN == "N")
{
VERW_STATUS = "N";
WS_ONLINE_U.STATRA = "0125";
WS_ONLINE_U.HERSTA = DBFOUT_CKRC;
//=         GO TO D100-999
}

D100_999:
return;
// ----------------------------------------------------------------------------
}

        public void D125_VESTIGINGSCONTROLE()
        {
        D125_010:
            LOG_WHEN_NOTFOUND_STATUS = "N";
            WS_CKR_NR = CKR270001I.CKR_NR;
            SQL04_S_VESTIG();
            if (SQL_FOUT)
            {
                //=         GO TO D125-999
            }
            if (SQL_NOTFOUND)
            {
                LOG_WHEN_NOTFOUND_STATUS = "N";
                SQL05_S_OMBUIG();
                if (SQL_FOUT)
                {
                    //=            GO TO D125-999
                }
                if (SQL_FOUND)
                {
                    LOG_WHEN_NOTFOUND_STATUS = "J";
                    WS_CKR_NR = WS_OMBUIG.CKR_NR;
                    SQL04_S_VESTIG();
                    if (SQL_FOUT)
                    {
                        //=               GO TO D125-999
                    }
                    CKR270001U.CKR_NR_OMBUIG = WS_OMBUIG.CKR_NR;
                    WS_ONLINE_U.STATRA = "0009";
                    WS_ONLINE_U.HERSTA = MELDING_CKRC;
                }
                else
                {
                    VERW_STATUS = "N";
                    WS_ONLINE_U.STATRA = "0002";
                    WS_ONLINE_U.HERSTA = DBFOUT_CKRC;
                    //=            GO TO D125-999
                }
            }
            else
            {
                //  Upshift waarde gebruiken
                WS_CKR_NR = WS_VESTIG.CKR_NR;
            }

        D125_999:
            return;
            // ----------------------------------------------------------------------------
        }

        public void D150_AUTORISATIE_CONTROLE()
        {
        D150_010:
            CKR3016I.USERID = WS_ONLINE_I.CODUSR;
            Process.Call("CKR3016C", CKR3016I, CKR3016U, CKR3010I);
            if (CKR3010I.CKR_FOUT != ZERO)
            {
                X666_ZEND_LOG();
                VERW_STATUS = "N";
                WS_ONLINE_U.STATRA = "0097";
                WS_ONLINE_U.HERSTA = HARDE_DBFOUT;
                //=         GO TO D150-999
            }

        D150_999:
            return;
            // ----------------------------------------------------------------------------
        }

        public void D200_HAAL_VERMELD()
        {
        D200_010:
            if (CKR270001I.CKR_NR == SPACES)
            {
                //  Record halen adhv TELECOM-NR
                SQL06_S_AVT_VERMELD();
                if (SQL_FOUT)
                {
                    //=            GO TO D200-999
                }
                if (SQL_NOTFOUND)
                {
                    VERW_STATUS = "N";
                    WS_ONLINE_U.STATRA = "0257";
                    WS_ONLINE_U.HERSTA = DBFOUT_CKRC;
                    //=            GO TO D200-999
                }
            }
            else
            {
                //  Zoeken op CKR-NR
                //  Eerst kijken of het CKR-nr uit de invoer voorkomt in OMBUIG
                SQL17_S_OMBUIG();
                if (SQL_FOUT)
                {
                    //=            GO TO D200-999
                }
                if (SQL_FOUND)
                {
                    CKR_NR_OMBUIG_STATUS = "J";
                }
                else
                {
                    CKR_NR_OMBUIG_STATUS = "N";
                }
                SQL09_O_CUR01_AVT_VERMELD();
                if (SQL_FOUT)
                {
                    //=            GO TO D200-999
                }
                INDICATOR_CUR01_AVT_VERMELD = "J";
                SQL10_F_CUR01_AVT_VERMELD();
                if (SQL_FOUT)
                {
                    //=            GO TO D200-999
                }
                if (SQL_NOTFOUND)
                {
                    //  Sluiten van cur02
                    SQL11_C_CUR01_AVT_VERMELD();
                    if (SQL_FOUT)
                    {
                        //=               GO TO D200-999
                    }
                    INDICATOR_CUR01_AVT_VERMELD = "N";
                    if (CKR_NR_NIET_IN_OMBUIG)
                    {
                        VERW_STATUS = "N";
                        WS_ONLINE_U.STATRA = "0257";
                        WS_ONLINE_U.HERSTA = DBFOUT_CKRC;
                        //  Er hoeft niet te worden gezocht met omgebogen CKR-nummers: zet cur03 op einde
                        STATUS_EOF_CUR02 = "J";
                    }
                    else
                    {
                        //  Openen van cursor voor zoeken met omgebogen ckr-nrs
                        SQL18_O_CUR02_AVT_VERMELD();
                        if (SQL_FOUT)
                        {
                            //=                  GO TO D200-999
                        }
                        INDICATOR_CUR02_AVT_VERMELD = "J";
                        SQL19_F_CUR02_AVT_VERMELD();
                        if (SQL_FOUT)
                        {
                            //=                  GO TO D200-999
                        }
                        if (SQL_NOTFOUND)
                        {
                            VERW_STATUS = "N";
                            WS_ONLINE_U.STATRA = "0257";
                            WS_ONLINE_U.HERSTA = DBFOUT_CKRC;
                            //  Sluiten van cur03
                            SQL20_C_CUR02_AVT_VERMELD();
                            INDICATOR_CUR02_AVT_VERMELD = "N";
                        }
                    }
                }
            }

        D200_999:
            return;
            // ----------------------------------------------------------------------------
        }

        public void D300_HAAL_VOLGENDE()
        {
        D300_010:
            if (CUR01_AVT_VERMELD_OPEN)
            {
                //  We zijn nog bezig om te zoeken met het CKR-nr uit de invoer
                SQL10_F_CUR01_AVT_VERMELD();
                if (SQL_FOUT)
                {
                    //=            GO TO D300-999
                }
                if (SQL_NOTFOUND)
                {
                    //  Niets meer gevonden: sluiten cursor
                    SQL11_C_CUR01_AVT_VERMELD();
                    if (SQL_FOUT)
                    {
                        //=               GO TO D300-999
                    }
                    INDICATOR_CUR01_AVT_VERMELD = "N";
                    if (CKR_NR_NIET_IN_OMBUIG)
                    {
                        //  Er hoeft niet te worden gezocht met omgebogen CKR-nummers: zet cur03 op einde
                        STATUS_EOF_CUR02 = "J";
                    }
                    else
                    {
                        //  Openen van cursor voor zoeken met omgebogen ckr-nrs
                        SQL18_O_CUR02_AVT_VERMELD();
                        if (SQL_FOUT)
                        {
                            //=                  GO TO D300-999
                        }
                        INDICATOR_CUR02_AVT_VERMELD = "J";
                        SQL19_F_CUR02_AVT_VERMELD();
                        if (SQL_FOUT)
                        {
                            //=                  GO TO D300-999
                        }
                        if (SQL_NOTFOUND)
                        {
                            //  Sluiten van cur03
                            SQL20_C_CUR02_AVT_VERMELD();
                            INDICATOR_CUR02_AVT_VERMELD = "N";
                        }
                    }
                }
            }
            else
            {
                //  We zijn aan het zoeken met CKR-nummers uit OMBUIG
                SQL19_F_CUR02_AVT_VERMELD();
                if (SQL_FOUT)
                {
                    //=            GO TO D300-999
                }
                if (SQL_NOTFOUND)
                {
                    //  Sluiten van cur03
                    SQL20_C_CUR02_AVT_VERMELD();
                    INDICATOR_CUR02_AVT_VERMELD = "N";
                }
            }

        D300_999:
            return;
            // ----------------------------------------------------------------------------
        }

        public void D400_VULLEN_CKR270001U()
        {
        D400_010:
            //  De antwoorden in de CKR270001U zetten
            CKR270001U(ANTWOORD_TELLER).ABONNEE_GEG_NR = WS_AVT_VERMELD.ABONNEE_GEG_NR;
            CKR270001U(ANTWOORD_TELLER).ADMIN_GRP = WS_AVT_VERMELD.ADMIN_GRP;
            CKR270001U(ANTWOORD_TELLER).TELECOM_NR = WS_AVT_VERMELD.TELECOM_NR;
            CKR270001U(ANTWOORD_TELLER).CKR_NR = WS_CKR_NR;
            CKR270001U(ANTWOORD_TELLER).B_NR_AFSCHERMING = WS_AVT_VERMELD.B_NR_AFSCHERMING;
            CKR270001U(ANTWOORD_TELLER).PUBLICATIE_CODE = WS_AVT_VERMELD.PUBLICATIE_CODE;
            CKR270001U(ANTWOORD_TELLER).NETWERK_TYPE = WS_AVT_VERMELD.NETWERK_TYPE;
            CKR270001U(ANTWOORD_TELLER).GEBRUIK_NETLIJN = WS_AVT_VERMELD.GEBRUIK_NETLIJN;
            CKR270001U(ANTWOORD_TELLER).DATUM_INGANG = WS_AVT_VERMELD.DATUM_INGANG;

        D400_999:
            return;
            // ----------------------------------------------------------------------------
        }

        public void D500_VULLEN_RESPBUF()
        {
        D500_010:
            //  Het vullen van de response buffer
            //  Aanvragen van een response buffer
            if ((VOLGNR_RESP_TELLER == 1) && (RESPONSE_REGELS == 1))
            {
                Process.Call("CKR3007C", CKR3007U, CKR3010I);
                if (CKR3010I.CKR_FOUT != ZERO)
                {
                    X666_ZEND_LOG();
                    VERW_STATUS = "N";
                    WS_ONLINE_U.STATRA = "0097";
                    WS_ONLINE_U.HERSTA = HARDE_DBFOUT;
                    //=            GO TO D500-999
                }
                WS_ONLINE_U.VERREC = "J";
                WS_DAT_RESP = DATUM_VANDAAG;
                WS_RESP_BUF_NR = CKR3007U.RESP_BUF_NR_NW;
                WS_ONLINE_U.NAAM_BUFFER = WS_NM_RESP_BUF;
            }
            CKRRESPBUF(RESPONSE_REGELS).ABONNEE_GEG_NR = WS_AVT_VERMELD.ABONNEE_GEG_NR;
            CKRRESPBUF(RESPONSE_REGELS).ADMIN_GRP = WS_AVT_VERMELD.ADMIN_GRP;
            CKRRESPBUF(RESPONSE_REGELS).TELECOM_NR = WS_AVT_VERMELD.TELECOM_NR;
            CKRRESPBUF(RESPONSE_REGELS).CKR_NR = WS_CKR_NR;
            CKRRESPBUF(RESPONSE_REGELS).B_NR_AFSCHERMING = WS_AVT_VERMELD.B_NR_AFSCHERMING;
            CKRRESPBUF(RESPONSE_REGELS).PUBLICATIE_CODE = WS_AVT_VERMELD.PUBLICATIE_CODE;
            CKRRESPBUF(RESPONSE_REGELS).NETWERK_TYPE = WS_AVT_VERMELD.NETWERK_TYPE;
            CKRRESPBUF(RESPONSE_REGELS).GEBRUIK_NETLIJN = WS_AVT_VERMELD.GEBRUIK_NETLIJN;
            CKRRESPBUF(RESPONSE_REGELS).DATUM_INGANG = WS_AVT_VERMELD.DATUM_INGANG;
            if ((RESPONSE_REGELS == AANTAL_OCCURS_270))
            {
                WS_RESPBUF.DAT_RESP = DATUM_VANDAAG;
                WS_RESPBUF.RESP_BUF_NR = CKR3007U.RESP_BUF_NR_NW;
                WS_RESPBUF.VOLGNR_RESP = VOLGNR_RESP_TELLER;
                //=         COMPUTE LENGTE-DATA = LENGTE-1-ANTWOORD-270 * RESPONSE-REGELS
                WS_RESPBUF.LNGTE_RESP_BUF = LENGTE_DATA;
                WS_RESPBUF.RESP_BUF_DATA = CKRRESPBUF;
                SQL16_I_RESPBUF();
                if (SQL_FOUT)
                {
                    //=            GO TO D500-999
                }
                //=         ADD  1 TO VOLGNR-RESP-TELLER
                RESPONSE_REGELS = 0;
            }

        D500_999:
            return;
            // ------------------------------------------------------------------------------
        }

        public void N100_CONTROLEER_HEADER()
        {
        N100_010:
            CKR_NET_HULPVELDEN.WS_COD_TRANS = "270";
            CKR_NET_HULPVELDEN.WS_COD_TRANS_VERSIE = "001";
            Process.Call("CKR3031C", WS_ONLINE_I);
            //=                            WS-COD-TRANS          OF CKR-NET-HULPVELDEN
            //=                            WS-COD-TRANS-VERSIE   OF CKR-NET-HULPVELDEN
            //=                            HERSTA OF WS-ONLINE-U
            //=                            STATRA OF WS-ONLINE-U
            if (WS_ONLINE_U.HERSTA != SPACE)
            {
                VERW_STATUS = "N";
            }

        N100_999:
            return;
            // ------------------------------------------------------------------------------
        }

        public void N200_CONTROLEER_TRANSACTIE()
        {
        N200_010:
            Process.Call("T270001I", WS_ONLINE_I.CKR_DEEL);
        //=                            HERSTA   OF WS-ONLINE-U
        //=                            STATRA   OF WS-ONLINE-U

        N200_999:
            return;
            // ------------------------------------------------------------------------------
        }

        public void N300_VUL_FOUTMELDING()
        {
        N300_010:
            //  Op dit punt wordt de fout-code t.b.v. loggen gevuld om te voorkomen dat
            //  de groepscode gelogd wordt i.p.v. de werkelijke foutcode.
            if (WS_ONLINE_U.HERSTA != SPACES)
            {
                CKR3039I.FOUT_CODE = string.Format("{0}{0}", WS_ONLINE_U.HERSTA, WS_ONLINE_U.STATRA);
            }
            else
            {
                CKR3039I.FOUT_CODE = SPACES;
            }
            //  Voorbereiding voor het eventueel vullen van het variabele deel van de
            //  foutmelding
            //=      INITIALIZE WS-MESSAGE
            if (WS_ONLINE_U.HERSTA == "W")
            {
                if (WS_ONLINE_U.STATRA == "0009")
                {
                    WS_MESSAGE = string.Format("{0}", CKR270001U.CKR_NR_OMBUIG);
                }
            }
            Process.Call("CKRALG03", WS_ONLINE_U.CKR_DEEL);
        //=                            WS-MESSAGE

        N300_999:
            return;
            // ************************************************* S Q L - R O U T I N E S ***
        }

        public void SQL04_S_VESTIG()
        {
        SQL04_010:
            LOG_WHEN_NOTFOUND_STATUS = "N";
            SQL_FOUND_STATUS = "J";
            CKR3010I.NM_BEST = "=VESTIG";
            CKR3010I.TEKST = "SQL04";
            //=       INITIALIZE WS-VESTIG
            Process.ExecSql(
            "SELECT  CKR_NR"
            + "INTO   :CKR-NR      OF WS-VESTIG FROM   =VESTIG"
            + "WHERE   CKR_NR = UPSHIFT(:WS-CKR-NR)"
            + "BROWSE ACCESS"
            );

        SQL04_999:
            return;
            // ------------------------------------------------------------------------------
        }

        public void SQL05_S_OMBUIG()
        {
        SQL05_010:
            LOG_WHEN_NOTFOUND_STATUS = "N";
            SQL_FOUND_STATUS = "J";
            CKR3010I.NM_BEST = "=OMBUIG";
            CKR3010I.TEKST = "SQL05";
            //=      INITIALIZE WS-OMBUIG
            Process.ExecSql(
            "SELECT CKR_NR"
            + "INTO   :CKR-NR OF WS-OMBUIG FROM   =OMBUIG"
            + "WHERE  CKR_NR_OUD = UPSHIFT(:WS-CKR-NR)"
            + "BROWSE ACCESS"
            );

        SQL05_999:
            return;
            // ------------------------------------------------------------------------------
        }

        public void SQL06_S_AVT_VERMELD()
        {
        SQL06_010:
            SQL_FOUND_STATUS = "J";
            LOG_WHEN_NOTFOUND_STATUS = "N";
            CKR3010I.NM_BEST = "=AVT_VERMELD";
            CKR3010I.TEKST = "SQL06";
            //=      INITIALIZE WS-AVT-VERMELD
            Process.ExecSql(
            "SELECT  ABONNEE_GEG_NR"
            + ", ADMIN_GRP"
            + ", TELECOM_NR"
            + ", CKR_NR"
            + ", B_NR_AFSCHERMING"
            + ", PUBLICATIE_CODE"
            + ", NETWERK_TYPE"
            + ", GEBRUIK_NETLIJN"
            + ", DATUM_INGANG"
            + ", DATUM_INVOER"
            + ", TIJD_INVOER"
            + ", COD_APPL_INVOER"
            + ", USERID_INVOER"
            + "INTO   :ABONNEE-GEG-NR      OF WS-AVT-VERMELD,"
            + ":ADMIN-GRP           OF WS-AVT-VERMELD,"
            + ":TELECOM-NR          OF WS-AVT-VERMELD,"
            + ":CKR-NR              OF WS-AVT-VERMELD,"
            + ":B-NR-AFSCHERMING    OF WS-AVT-VERMELD,"
            + ":PUBLICATIE-CODE     OF WS-AVT-VERMELD,"
            + ":NETWERK-TYPE        OF WS-AVT-VERMELD,"
            + ":GEBRUIK-NETLIJN     OF WS-AVT-VERMELD,"
            + ":DATUM-INGANG        OF WS-AVT-VERMELD,"
            + ":DATUM-INVOER        OF WS-AVT-VERMELD,"
            + ":TIJD-INVOER         OF WS-AVT-VERMELD,"
            + ":COD-APPL-INVOER     OF WS-AVT-VERMELD,"
            + ":USERID-INVOER       OF WS-AVT-VERMELD FROM   =AVT_VERMELD"
            + "WHERE  TELECOM_NR = :TELECOM-NR OF CKR270001I AND    ADMIN_GRP IN"
            + "(SELECT ADMIN_GRP FROM   =AVT_ADMINAPP"
            + "WHERE  COD_APPL = :WS-COD-APPL"
            + "BROWSE ACCESS)"
            + "BROWSE ACCESS"
            );

        SQL06_999:
            return;
            // -----------------------------------------------------------------------------
        }

        public void SQL09_O_CUR01_AVT_VERMELD()
        {
        SQL09_010:
            //  Het openen van de cursor cur02 op =AVT_VERMELD
            SQL_FOUND_STATUS = "J";
            LOG_WHEN_NOTFOUND_STATUS = "N";
            CKR3010I.NM_BEST = "=AVT_VERMELD";
            CKR3010I.TEKST = "SQL09";
            Process.ExecSql(
            "OPEN CUR01_AVT_VERMELD"
            );

        SQL09_999:
            return;
            // ------------------------------------------------------------------------------
        }

        public void SQL10_F_CUR01_AVT_VERMELD()
        {
        SQL10_010:
            SQL_FOUND_STATUS = "J";
            LOG_WHEN_NOTFOUND_STATUS = "N";
            CKR3010I.NM_BEST = "=AVT_VERMELD";
            CKR3010I.TEKST = "SQL10";
            //=      INITIALIZE WS-AVT-VERMELD
            Process.ExecSql(
            "FETCH CUR01_AVT_VERMELD"
            + "INTO   :ABONNEE-GEG-NR      OF WS-AVT-VERMELD,"
            + ":ADMIN-GRP           OF WS-AVT-VERMELD,"
            + ":TELECOM-NR          OF WS-AVT-VERMELD,"
            + ":CKR-NR              OF WS-AVT-VERMELD,"
            + ":B-NR-AFSCHERMING    OF WS-AVT-VERMELD,"
            + ":PUBLICATIE-CODE     OF WS-AVT-VERMELD,"
            + ":NETWERK-TYPE        OF WS-AVT-VERMELD,"
            + ":GEBRUIK-NETLIJN     OF WS-AVT-VERMELD,"
            + ":DATUM-INGANG        OF WS-AVT-VERMELD,"
            + ":DATUM-INVOER        OF WS-AVT-VERMELD,"
            + ":TIJD-INVOER         OF WS-AVT-VERMELD,"
            + ":COD-APPL-INVOER     OF WS-AVT-VERMELD,"
            + ":USERID-INVOER       OF WS-AVT-VERMELD"
            );
            if (SQL_NOTFOUND)
            {
                STATUS_EOF_CUR01 = "J";
            }

        SQL10_999:
            return;
            // -----------------------------------------------------------------------------
        }

        public void SQL11_C_CUR01_AVT_VERMELD()
        {
        //  Sluit de cursor cur02 op =AVT_VERMELD
        SQL11_010:
            SQL_FOUND_STATUS = "J";
            LOG_WHEN_NOTFOUND_STATUS = "N";
            CKR3010I.NM_BEST = "=AVT_VERMELD";
            CKR3010I.TEKST = "SQL11";
            Process.ExecSql(
            "CLOSE CUR01_AVT_VERMELD"
            );

        SQL11_999:
            return;
            // -----------------------------------------------------------------------------
        }

        public void SQL16_I_RESPBUF()
        {
        //  Invoeren van records in de response buffer tabel
        SQL16_010:
            SQL_FOUND_STATUS = "J";
            LOG_WHEN_NOTFOUND_STATUS = "N";
            CKR3010I.NM_BEST = "=RESPBUF";
            CKR3010I.TEKST = "SQL16";
            Process.ExecSql(
            "INSERT INTO =RESPBUF"
            + "(  DAT_RESP       ,"
            + "RESP_BUF_NR    ,"
            + "VOLGNR_RESP    ,"
            + "LNGTE_RESP_BUF ,"
            + "RESP_BUF_DATA)"
            + "VALUES ( :DAT-RESP         OF WS-RESPBUF,"
            + ":RESP-BUF-NR      OF WS-RESPBUF,"
            + ":VOLGNR-RESP      OF WS-RESPBUF,"
            + ":LNGTE-RESP-BUF   OF WS-RESPBUF,"
            + ":RESP-BUF-DATA    OF WS-RESPBUF)"
            );

        SQL16_999:
            return;
            // ------------------------------------------------------------------------------
        }

        public void SQL17_S_OMBUIG()
        {
        SQL17_010:
            SQL_FOUND_STATUS = "J";
            LOG_WHEN_NOTFOUND_STATUS = "N";
            CKR3010I.NM_BEST = "=OMBUIG";
            CKR3010I.TEKST = "SQL17";
            //=      WS-DUMMY-CKRNR
            Process.ExecSql(
            "SELECT MIN(CKR_NR_OUD)"
            + "INTO   :WS-DUMMY-CKRNR FROM   =OMBUIG"
            + "WHERE  CKR_NR = UPSHIFT(:CKR-NR OF CKR270001I)"
            + "BROWSE ACCESS"
            );

        SQL17_999:
            return;
            // -----------------------------------------------------------------------------
        }

        public void SQL18_O_CUR02_AVT_VERMELD()
        {
        SQL18_010:
            //  Het openen van de cursor cur03 op =AVT_VERMELD
            SQL_FOUND_STATUS = "J";
            LOG_WHEN_NOTFOUND_STATUS = "N";
            CKR3010I.NM_BEST = "=AVT_VERMELD";
            CKR3010I.TEKST = "SQL18";
            Process.ExecSql(
            "OPEN CUR02_AVT_VERMELD"
            );

        SQL18_999:
            return;
            // ------------------------------------------------------------------------------
        }

        public void SQL19_F_CUR02_AVT_VERMELD()
        {
        SQL19_010:
            SQL_FOUND_STATUS = "J";
            LOG_WHEN_NOTFOUND_STATUS = "N";
            CKR3010I.NM_BEST = "=AVT_VERMELD";
            CKR3010I.TEKST = "SQL19";
            //=      INITIALIZE WS-AVT-VERMELD
            Process.ExecSql(
            "FETCH CUR02_AVT_VERMELD"
            + "INTO   :ABONNEE-GEG-NR      OF WS-AVT-VERMELD,"
            + ":ADMIN-GRP           OF WS-AVT-VERMELD,"
            + ":TELECOM-NR          OF WS-AVT-VERMELD,"
            + ":CKR-NR              OF WS-AVT-VERMELD,"
            + ":B-NR-AFSCHERMING    OF WS-AVT-VERMELD,"
            + ":PUBLICATIE-CODE     OF WS-AVT-VERMELD,"
            + ":NETWERK-TYPE        OF WS-AVT-VERMELD,"
            + ":GEBRUIK-NETLIJN     OF WS-AVT-VERMELD,"
            + ":DATUM-INGANG        OF WS-AVT-VERMELD,"
            + ":DATUM-INVOER        OF WS-AVT-VERMELD,"
            + ":TIJD-INVOER         OF WS-AVT-VERMELD,"
            + ":COD-APPL-INVOER     OF WS-AVT-VERMELD,"
            + ":USERID-INVOER       OF WS-AVT-VERMELD"
            );
            if (SQL_NOTFOUND)
            {
                STATUS_EOF_CUR02 = "J";
            }

        SQL19_999:
            return;
            // -----------------------------------------------------------------------------
        }

        public void SQL20_C_CUR02_AVT_VERMELD()
        {
        //  Sluit de cursor cur03 op =AVT_VERMELD
        SQL20_010:
            SQL_FOUND_STATUS = "J";
            LOG_WHEN_NOTFOUND_STATUS = "N";
            CKR3010I.NM_BEST = "=AVT_VERMELD";
            CKR3010I.TEKST = "SQL20";
            Process.ExecSql(
            "CLOSE CUR02_AVT_VERMELD"
            );

        SQL20_999:
            return;
            // ------------------------------------------------------------------------------
        }

        public void X666_ZEND_LOG()
        {
        //  Bij ernstige fouten moet de beheerder op de hoogte worden gesteld.
        //  Dit doen we via een LOG-proces. CKR3010I is het transactierecord dat naar de
        //  LOG-ger wordt gestuurd. COD-TRANS bevat het nummer van de server waarin de
        //  fout ontstaat.
        //  Het is op dit punt niet altijd zeker dat de LOG-FILE open is.
        X666_010:
            CKR3010I.COD_TRANS = WS_ONLINE_I.CODTRA;
            CKR3010I.COD_SYS = WS_ONLINE_I.CODSYS;
            CKR3010I.COD_APPL = WS_ONLINE_I.CODAPL;
            CKR3010I.USERID = WS_ONLINE_I.CODUSR;
            CKR3010I.COD_HER = WS_COD_HER;
            CKR3010I.INVOER_DATA = WS_ONLINE_I.INFTXT;
            //    Het maken van een "nette" foutmelding ivm de versie-ondersteuning
            HULP_VERSIE = WS_ONLINE_I.CODTRAVER;
            HULP_TEKST = CKR3010I.TEKST;
            if (CKR270001I.CKR_NR != SPACES)
            {
                HULP_TEKST_2 = CKR270001I.CKR_NR;
            }
            else
            {
                HULP_TEKST_2 = CKR270001I.TELECOM_NR;
            }
            //     STRING "Versie: "         DELIMITED BY SIZE HULP-VERSIE        DELIMITED BY SIZE "/"                DELIMITED BY SIZE HULP-TEKST         DELIMITED BY SPACE "/"                DELIMITED BY SIZE "Zoekarg:"         DELIMITED BY SIZE HULP-TEKST-2       DELIMITED BY SIZE
            //=     INTO TEKST OF CKR3010I
            Process.Call("CKR3010C", CKR3010I);
            CKR3010I.TEKST = SPACES;
        //=                     NM-BEST OF CKR3010I
        //=                     HULP-VERSIE
        //=                     HULP-TEKST

        X666_999:
            return;
            // ----------------------------------------------------------------------------
        }

        public void X777_LOG_TRANSACTIE()
        {
        X777_010:
            CKR3039I.COD_APPL = WS_ONLINE_I.CODAPL;
            CKR3039I.COD_TRANS = WS_ONLINE_I.CODTRA;
            CKR3039I.VERSIE = WS_ONLINE_I.CODTRAVER;
            CKR3039I.USERID = WS_ONLINE_I.CODUSR;
            CKR3039I.DAT_AANROEP = DATUM_VANDAAG;
            CKR3039I.IND_BATCH_ONLINE = WS_COD_HER;
            CKR3039I.CKR_NR = WS_CKR_NR;
            Process.Call("CKR3039C", CKR3039I, CKR3010I);
            if (CKR3010I.CKR_FOUT != ZERO)
            {
                if (CKR3010I.TEKST == SPACES)
                {
                    CKR3010I.TEKST = "Fout in module 30.39";
                }
                X666_ZEND_LOG();
            }

        X777_999:
            return;
            // ********************************************* E R R O R - R O U T I N E S**
        }

        public void X900_NOTFOUND()
        {
        //  Wordt door SQL aangeroepen als er een "NOTFOUND" optreedt.
        //  Als LOG-WHEN-NOTFOUND aan staat, is er een ernstige fout en moet de notfound
        //  worden gemeld bij de beheerder onder vermelding van foutcode 7 en SQLCA
        X900_010:
            if (LOG_WHEN_NOTFOUND)
            {
                CKR3010I.SQLCA_AREA = SQLCA;
                CKR3010I.CKR_FOUT = 7;
                SQL_STATUS = "N";
                VERW_STATUS = "N";
                WS_ONLINE_U.STATRA = "0097";
                WS_ONLINE_U.HERSTA = HARDE_DBFOUT;
                X666_ZEND_LOG();
            }
            SQL_FOUND_STATUS = "N";

        X900_999:
            return;
            // ----------------------------------------------------------------------------
        }

        public void X910_SQLERROR()
        {
        //  Wordt door SQL aangeroepen als er een ernstige fout optreedt.
        //  Zendt CKR3010I naar de logger en stelt globale variabelen SQL-STATUS en
        //  VERW-STATUS in.
        X910_010:
            //     In bepaalde gevallen geeft SQL bij een NOTFOUND situatie niet de normale
            //     code 100 terug maar wordt geprobeerd de velden te vullen met NULL. Dit nu
            //     lukt niet want de database is gedefinieerd met NOT NULL. Dit heeft tot
            //     gevolg dat SQL een fatal error geeft (-8423), terwijl het dus eigenlijk
            //     een normale NOTFOUND situatie is.
            if (ERRCODE(1) == _8423)
            {
                X900_NOTFOUND();
                goto X910_999;
            }
            CKR3010I.SQLCA_AREA = SQLCA;
            //     Foutcode 1: fout in de database.
            CKR3010I.CKR_FOUT = 1;
            X666_ZEND_LOG();
            SQL_STATUS = "N";
            VERW_STATUS = "N";
            WS_ONLINE_U.STATRA = "0097";
            WS_ONLINE_U.HERSTA = HARDE_DBFOUT;

        X910_999:
            return;
            // ----------------------------------------------------------------------------
        }

        public void X920_SQLWARNING()
        {
        X920_010:
        //=      CONTINUE

        X920_999:
            return;
            // ******************************** E I N D E *********************************
        }
    }
}
