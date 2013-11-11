function QuerySetFocus(element) {
    var elemLen = element.value.length;
    // For IE Only
    if (document.selection) {
        // Set focus
        element.focus();
        // Use IE Ranges
        var oSel = document.selection.createRange();
        // Reset position to 0 & then set at end
        oSel.moveStart('character', -elemLen);
        oSel.moveStart('character', elemLen);
        oSel.moveEnd('character', 0);
        oSel.select();
    }
    else if (element.selectionStart || element.selectionStart == '0') {
        // Firefox/Chrome
        element.selectionStart = elemLen;
        element.selectionEnd = elemLen;
        element.focus();
    } // if
} // SetCaretAtEnd()

var delay = (function () {
    var timer = 0;
    return function (callback, ms) {
        clearTimeout(timer);
        timer = setTimeout(callback, ms);
    };
})();

function Query_GridExtender_Init() {
    $('.data-query-textFilter').each(function () { Query_TextField_Init($(this)); });
    $('.data-query-datepicker').each(function () { Query_DateField_Init($(this)); });
    $('.data-query-dropdown').each(function () { Query_DropDownField_Init($(this)); });
}

function Query_TextField_Init($element) {
    var currentVal = $element.val();
    var delayMs = $element.attr('data-query-filterDelay');
    var postbackName = $element.attr('data-query-postbackName');
    var focus = $element.attr('data-query-focus');
    var defaultValue = $element.attr('data-query-placeholder');
    
    // Store initial value
    if (defaultValue == currentVal) {
        currentVal = "";
    }
    $element.attr("data-query-value", currentVal);
    
    // Restore focus
    focus = focus ? focus.toLowerCase() : 'false';
    if (focus == 'true') {
        QuerySetFocus($element[0]);
    }

    $element.keyup(function (e) {
        // If the user pressed the ENTER key (13) make a postback
        var theKey;
        e = (window.event) ? event : e;
        theKey = (e.keyCode) ? e.keyCode : e.charCode;

        document.getElementById('__LASTFOCUS').value = this.name;
        if (theKey == "13") { // 13 ENTER key
            __doPostBack(postbackName, '');
        }
        
        // If no delay was defined we are done
        if (delayMs == undefined) {
            return;
        }

        // Set timer for auto postback
        var oldVal = $(this).attr('data-query-value');
        var val = $(this).val();

        // Ignore default value (placeholder)
        if (val === defaultValue) {
            val = "";
        }

        // skip postback if value did not change
        if (oldVal != val) {
            $(this).attr('data-query-value', val);
            delay(function () {
                __doPostBack(postbackName, '');
            }, delayMs);
        }
    });
}

function Query_DateField_Init($element) {
    Query_TextField_Init($element);
}

function Query_DateField_CancelShowWhenFocus(sender, args) {
    var focus = $(sender._element).attr('data-query-focus');

    focus = focus ? focus.toLowerCase() : 'false';
    if (focus == 'true') {
        args.set_cancel(true);
    }
}

function Query_DropDownField_Init($element) {
    var focus = $element.attr('data-query-focus');
    // Restore focus
    focus = focus ? focus.toLowerCase() : 'false';
    if (focus == 'true') {
        $element[0].focus();
    }
    
    $element.change(function() {
        document.getElementById('__LASTFOCUS').value = this.name;
        __doPostBack($(this).attr('data-query-postbackName'), '');
    });
}