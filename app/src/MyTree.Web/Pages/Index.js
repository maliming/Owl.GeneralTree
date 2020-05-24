$(function () {

    $('#tree')
        .on('move_node.jstree', function (e, data) {
            var beforeId = data.instance.get_node(data.parent).children[data.position + 1];
            if (beforeId) {
                myTree.region.moveToBefore(data.node.id, beforeId).then(function () {
                    $('#tree').jstree("refresh");
                    $('#treeClone').jstree("refresh");
                });
            } else {
                var parentId = data.parent === "#" ? null : data.parent;
                myTree.region.moveTo({
                    "id": data.node.id,
                    "parentId": parentId
                }).then(function () {
                    $('#tree').jstree("refresh");
                });
            }

        })
        .jstree({
            'core': {
                'data': function (obj, callback) {
                    myTree.region.getTrees({}).done(function (trees) {
                        var mapToJsTreeNode = function (values) {
                            var map = function (value) {
                                return {
                                    id: value.id,
                                    text: value.name + " [" + value.code + "]",
                                    children: mapToJsTreeNode(value.children),
                                    state: {
                                        opened: true
                                    },
                                };
                            }

                            if (values) {
                                if (_.isArray(values)) {
                                    return _.map(values, function (value) {
                                        return map(value);
                                    });
                                } else {
                                    return map(values);
                                }
                            }
                        };

                        if (_.isArray(trees)) {
                            callback.call(this, mapToJsTreeNode(trees));
                        } else {

                        }
                    });

                },
                "check_callback": true
            },
            "contextmenu": {
                "items": function (node) {
                    return {
                        "Rename": {
                            "label": "Rename",
                            "action": function (obj) {
                                alert("rename")
                            }
                        },
                        "Delete": {
                            "label": "Delete",
                            "action": function (obj) {
                                myTree.region.delete(node.id).done(function () {
                                    $('#tree').jstree("refresh");
                                })
                            }
                        },
                        "Regenerate": {
                            "label": "Regenerate",
                            "action": function (obj) {
                                myTree.region.regenerate({"parentId": node.id}).done(function () {
                                    $('#tree').jstree("refresh");
                                })
                            }
                        },
                    };
                }
            },
            "plugins": ["dnd", "contextmenu"]
        });

    $("#RegenerateTree").click(function () {
        myTree.region.regenerate({}).done(function () {
            $('#tree').jstree("refresh");
        })
    })

    $("#ReseedTree").click(function () {
        myTree.region.reseed({}).done(function () {
            $('#tree').jstree("refresh");
        })
    })

});
