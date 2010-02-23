function appendAnchor(anchorName, element) {
    $('<a name="' + anchorName + '"></a>').prependTo(element);
}

$(document).ready(function() {
    $('hr').each(function(i, hr) {
        $('<p><a href="#TopOfPage">Back to top...</a><br></br><br></br></p>').replaceAll(hr);
    });

    var top = $('<div><a name="TopOfPage" /></div>').prependTo('body').get(0);

    var title = 'StructureMap  -  ' + $('title').html();
    $('<h1></h1>').html(title).appendTo(top);

    $('h1, h2, h3, h4').addClass('outline');
    var outline = $('<ul></ul>').appendTo(top).get(0);

    var headerContainer = null;
    $('.outline').each(function(idx, header) {
        var link = "section" + idx;
        var html = '<li><a href="#' + link + '">' + header.innerHTML + '</a></li>';

        if (header.tagName == 'H2') {
            var headerBullet = $(html).appendTo(outline).get(0);
            headerContainer = $('<ul></ul>').appendTo(headerBullet).get(0);
        }
        else {
            $(html).appendTo(headerContainer);
        }


        appendAnchor(link, header);
    });

    $('<hr />').appendTo(top);
});
		