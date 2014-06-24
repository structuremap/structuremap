$(document).ready(function () {
    $('.prettyprint').each(function (i, pre) {
        var linenum = $(pre).attr('data-linenums');
        if (linenum) {
            $(pre).addClass('linenums:' + linenum);
        }
    });

    prettyPrint();
});