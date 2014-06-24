$(document).ready(function () {
    $('#topic-tree').nestable({ group: 1 });
    $('#new-leaf').nestable({ group: 1 });

    $('#topics-tab').click();

    var adderView = new TopicAdderView();
    var controller = new TopicController(adderView);

    var activateLeaf = function(li) {
        var leaf = new TopicLeaf(li);
        controller.select(leaf);

        $('#pin-current').show();

        var plugin = $('#topic-tree').data("nestable");
        plugin.expandItem($(this));
    };

    $('#topic-tree').on('click', 'span.topic-title', function (e) {
        var li = $(this).closest('li.dd-item');
        activateLeaf(li);


        e.stopImmediatePropagation();
    });

    /*
    $('#pin-current').click(function () {
        var leaf = $(controller.current).get(0);
        while (leaf.sib)
        
        $('#unpin').show();
        $('#pin-current').hide();
    });

    $('#unpin').click(function() {
        $('li.dd-item').show();
        $('#unpin').hide();
        $('#pin-current').show();
    });
    */

    $('#add-topic-button').click(function(e) {
        controller.addTopic();
        e.stopImmediatePropagation();
    });

    $('#topic-tree').on('click', 'a.close', function(e) {
        $(this).closest('li.dd-item').remove();

        e.stopImmediatePropagation();
    });

    $('#reset').click(function() {
        window.location.href = window.location.href;
    });

    $('#submit').click(function () {
        var clientModel = $('#topic-tree').nestable('serialize');
        var serverModel = transformClientModel(clientModel[0]);

        var url = $(this).attr('data-url');
        var json = window.JSON.stringify(serverModel);

        $('#submit').attr('disabled', 'true');

        $.ajax({            
           type: "POST",
           url: url,
           data: json,
           contentType: 'text/json',
           success: function(data, status, xhr) {
               $("#alert").show();
               $('#tabs').hide();
               $('.tab-content').hide();
           },
           
           error: function(data, status, xhr) {
               alert("The topic update failed.  Check the console window");
               $('#submit').attr('disabled', 'false');
           }
        });
   });
    


    $('#expand-all').click(function() {
        $('#topic-tree').nestable("expandAll");
    });

    $('#collapse-all').click(function () {
        $('#topic-tree').nestable("collapseAll");
    });

    $('#topic-tree').nestable("collapseAll");
    var firstItem = $('li.dd-item:first');
    var plugin = $('#topic-tree').data("nestable");
    plugin.expandItem(firstItem);

    activateLeaf(firstItem);
});

function transformClientModel(client) {
    var server = { Url: client.url, Title: client.title, Id: client.id, Key: client.key, Children: [] };
    for (var i = 0; i < client.children.length; i++) {
        var serverChild = transformClientModel(client.children[i]);
        server.Children.push(serverChild);
    }

    return server;
};

function TopicAdderView() {
    var self = this;

    $('#topic-editing-content').hide();

    var form = $('#add-topic-form');
    

    var showExistingButtons = function() {
        $('.input-append a', existingEditor.form).show();
    };

    var hideExistingButtons = function() {
        $('.input-append a', existingEditor.form).hide();
    };

    var existingEditor = new EditorPane($('#inline-editor'));
    $('input', existingEditor.form).change(showExistingButtons);

    $('#change-current-topic').click(function(e) {
        existingEditor.commit();
        hideExistingButtons();
        e.stopImmediatePropagation();
    });


    $('#reset-current-topic').click(function (e) {
        existingEditor.reset();
        hideExistingButtons();
        e.stopImmediatePropagation();
    });

    var editor = new EditorPane(form);
    var newLeaf = new TopicLeaf($('#new-leaf'));
    editor.edit(newLeaf);

    self.activate = function(leaf) {
        $('#topic-editing-content').show();
        $('#inline-editor').show();
        existingEditor.edit(leaf);

        if (leaf.get('key') == 'index') {
            
        } else {
            $('input', existingEditor.item)
        }

        hideExistingButtons();

        $('.parent-title').html(leaf.get('title'));

        self.resetNewTopicForm();
    };

    self.resetNewTopicForm = function() {
        editor.clear();
    };

    self.buildLeaf = function() {
        editor.commit();

        return newLeaf.clone();
    };

    self.focusOnKey = function() {
        $('.key', form).focus();
    };

    return self;
}

function TopicController(adder) {
    var self = this;

    self.current = null;

    self.select = function (leaf) {
        if (self.current != null) {
            self.current.markInactive();
        }

        self.current = leaf;
        leaf.markActive();

        adder.activate(leaf);
    };

    self.addTopic = function () {
        var newLeaf = adder.buildLeaf();
        if (!newLeaf.get('key') || newLeaf.get('key') == '') {
            alert("'Key' is required");
            adder.focusOnKey();
            return;
        }
        
        if (!newLeaf.get('title') || newLeaf.get('title') == '') {
            newLeaf.set('title', newLeaf.get('key'));
        }

        if (!newLeaf.get('url') || newLeaf.get('url') == '') {
            newLeaf.set('url', newLeaf.get('key'));
        }

        newLeaf.appendTo(self.current);
        adder.resetNewTopicForm();
    };

    return self;
}

function replaceAll(find, replace, str) {
    return str.replace(new RegExp(find, 'g'), replace);
}

function EditorPane(form) {
    var self = this;
    self.form = form;


    var keyInput = $('.key', form);
    var titleInput = $('.title', form);
    var urlInput = $('.url', form);
    titleInput.change(function() {
        if (titleInput.val() == '') return;

        var newKey = replaceAll(" ", "-", titleInput.val().toLowerCase());

        if (keyInput.val() == '') {
            keyInput.val(newKey);
        }
        
        if (urlInput.val() == '') {
            urlInput.val(newKey);
        }
    });

    keyInput.change(function() {
        if (urlInput.val() == '') {
            urlInput.val(keyInput.val());
        }
    });

    self.edit = function (leaf) {
        self.leaf = leaf;
        self.reset();

        $('input', form).prop('disabled', leaf.isIndex());
    };

    self.push = function(prop) {
        var val = $('.' + prop, form).val();
        self.leaf.set(prop, val);
    };

    self.pull = function(prop) {
        var val = self.leaf.get(prop);
        $('.' + prop, form).val(val);
    };

    self.commit = function() {
        self.push('title');
        self.push('url');
        self.push('key');
    };

    self.reset = function() {
        self.pull('title');
        self.pull('url');
        self.pull('key');
    };

    self.clear = function () {
        $('.title', form).val('').focus();
        $('.url', form).val('');
        $('.key', form).val('');
        $('.sections', form).val('');
    };

    return self;
}

// Wraps the <li> for a single item
function TopicLeaf(item) {
    var self = this;
    
    self.item = item;

    self.clone = function() {
        var clonedNode = $(self.item).get(0).cloneNode(true);
        return new TopicLeaf(clonedNode);
    };

    self.get = function(key) {
        return $(item).attr('data-' + key);
    };

    self.set = function(key, value) {
        $(self.item).attr('data-' + key, value);
        
        if (key == 'title') {
            $('.topic-title:first', self.item).html(value);
        }
    };

    self.markActive = function() {
        $(item).addClass('active-topic');
    };

    self.markInactive = function() {
        $(item).removeClass('active-topic');
    };

    self.appendTo = function (parent) {
        var li = $(item).get(0);
        $('ol', parent.item).get(0).appendChild(li);

        var plugin = $('#topic-tree').data("nestable");
        plugin.setParent($(li));
    };

    self.isIndex = function() {
        return self.get('key').toLowerCase() == 'index';
    };

    return self;
}

