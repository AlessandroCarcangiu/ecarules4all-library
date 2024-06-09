File in cui inserire note su decisioni prese per la trascrizione di metodi all'interno di RuleUtils.

Classe ButtonsHandle:

    Decisioni:

        Metodo AddAction:
            Decisiione di non spostare il metodo all'interno della libreria in quanto composto da un'unica riga di 
            codice e non risulterebbe utile in alcun modo il suo spostamento.
    
        Metodo AddCondition:
            Suddivisione del metodo e spostamento delle porzioni di codice di logica in RuleUtils. Tre nuovi metodi che 
            andranno richiamati all'interno del metodo originale in sostituzione alle porzioni di codice che sono state
            spostate. Nuovi metodi: SimpleConditionExists, InstantiateCompositeCondition, ActivateSimpleCondition.
    
        Metodo FindAction:
            Suddivisione del metodo e spostamento delle porzioni di codice di logica in RuleUtils. Tre nuovi metodi che
            andranno richiamati all'interno del metodo originale in sostituzione alle porzioni di codice che sono state
            spostate. Non spostamento di una porzione di codice specifica a causa della mancata implementazione di 
            ECACamera all'interno della libreria. Nuovi metodi: HandleVerbSelectedType, HandleInputField, 
            HandleInputField.
    
        Metodo compositeConditionExists:
            Spostamento del metodo intero all'interno della libreria e sua rinominazione in CompositeConditionExists.
    
        Metodo FindSimpleCondition:
            Decisione di non spostare alcuna porzione di codice all'interno della libreria: il metodo fa utilizzo di 
            vari elementi UI come ConditionDropdownHandler (e derivati dalla classe in questione) e ECACamera. 
            La divisione del metodo risulterebbe inutile. 
    
        Metodo CreateStringCondition:
            Decisione di non spostare alcuna porzione di codice all'interno della libreria: il metodo fa utilizzo di 
            vari elementi UI come ConditionDropdownHandler, Dropdown e InputField in tutto il codice del metodo.

        Metodo findOperator: 
            Decisione di non spostare alcuna porzione di codice all'interno della libreria: il metodo è composto da 
            poche righe di codice che comunque svolgono una elaborazione utilizzando elementi UI come Dropdown e 
            ConditionDropdownHandler. 

        Medodo CreateCompositeCondition:
            Decisione di suddividere il metodo in parti e spostarne alcune all'interno di RuleUtils nella libreria. 
            Il metodo fa uso dei metodi FindSimpleCondition e findOperator (entrambi UI), dunque tale porzione di 
            codice dovrà necessariamente rimanere nel codice Unity. 
            Nuovi metodi della libreria: ReverseConditionsAndTypes (da rivedere, sono due righe) e 
            CreateCompositeConditionFromLists.

        Metodo CreateRule:
            Decisione di suddividere il metodo in parti e spostare tali parti all'interno di RuleUtils nella libreria.
            Il metodo originale resta comunque all'interno del progetto originale e farà riferimento ai nuovi metodi
            di libreria IsValidWhenAction e CreateFinalRule. La parte che rimarrà nel progetto Unity fa utilizzo dei 
            metodi UI FindAction, FindSimpleCondition, CreateStringCondition, e CreateCompositeCondition.

        Metodo ClearEventAction:
            Decisione di non spostare alcuna porzione di codice all'interno della libreria: questo metodo fa ampio
            utilizzo di classi ed elementi della UI e non è presente logica importabile in RuleUtils.

        Metodo DiscardChanges:
            Decisione di non spostare alcuna porzione di codice all'interno della libreria: il metodo fa ampio utilizzo
            di elementi UI come il metodo ClearEventAction e della classe ConditionDropdownHandler. Non ci sono grandi
            porzioni di codice che potrebbero risultare utili nella libreria.

        Metodo RemoveAction:
            Boh è un debug.log, non penso sia utile nella libreria in alcun modo.

        Metodo ParseActionEvent: 
            Decisione di non spostare alcuna porzione di codice all'interno della libreria: il metodo fa ampio utilizzo
            della classe Dropdown (UI) e la sfrutta per le sue elaborazioni.

        Metodo ParseSimpleCondition: 
            Decisione di non spostare alcuna porzione di codice all'interno della libreria: il metodo fa ampio utilizzo
            della classe Dropdown (UI) e la sfrutta per le sue elaborazioni.
            
        Metodo ParseCompositeCondition: 
            Decisione di non spostare alcuna porzione di codice all'interno della libreria: il metodo fa utilizzo della 
            classe Dropdown (UI) e la sfrutta per le sue elaborazioni oltre che utilizzare il metodo 
            ParseSimpleCondition (UI).

        Metodo ParseMultipleActions: 
            Decisione di non spostare alcuna porzione di codice all'interno della libreria: il metodo fa utilizzo dei
            metodi FindAction e ParseActionEvent, entrambi facenti parte della UI.

        Metodo FormatRule:
            Decisione di non spostare alcuna porzione di codice all'interno della libreria: il metodo fa ampio utilizzo
            di metodi UI come FindAction, FindSimpleCondition, ParseSimpleCondition, ParseCompositeCondition e 
            ParseMultipleActions in ogni parte del suo codice, rendendone impossibile la suddivisione. Modifica: il
            metodo compositeConditionExists viene sostituito dal metodo di libreria CompositeConditionExists. 

        Metodo CreateRuleRow:
            Spostamento di una porzione di codice all'interno di un nuovo metodo facente parte della libreria in 
            RuleUtils. Il metodo in questione, FormatRuleLabel, formatta una nuova regola (label). Da verificare la
            reale utilità nella libreria. Il resto del metodo fa utilizzo di variabili della classe Text (UI).