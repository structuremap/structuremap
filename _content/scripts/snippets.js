$(document).ready(function () {
    $('a.edit-snippet').click(function (e) {
        $.ajax({
            type: "POST",
            url: $(this).data('url'),
            data: { Name: $(this).data('name'), Bottle: $(this).data('bottle') },
            dataType: 'json'
        });

        e.preventDefault();
        return false;
    });

    $('a.snippet-link').click(function (e) {
        var url = $(this).data('url');

        $('#snippetModalBody').load(url, null, function () {
            prettyPrint();
        });

        $('#snippetModalBody .accordion').collapse();
        $('#snippetModal').modal('show');



        e.preventDefault();
        return false;
    });
});