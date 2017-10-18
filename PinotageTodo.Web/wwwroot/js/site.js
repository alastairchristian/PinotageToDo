/*global jQuery, Handlebars, Router */
jQuery(function ($) {
    'use strict';

    Handlebars.registerHelper('eq', function (a, b, options) {
        return a === b ? options.fn(this) : options.inverse(this);
    });

    var ENTER_KEY = 13;
    var ESCAPE_KEY = 27;

    var util = {
        uuid: function () {
            /*jshint bitwise:false */
            var i, random;
            var uuid = '';

            for (i = 0; i < 32; i++) {
                random = Math.random() * 16 | 0;
                if (i === 8 || i === 12 || i === 16 || i === 20) {
                    uuid += '-';
                }
                uuid += (i === 12 ? 4 : (i === 16 ? (random & 3 | 8) : random)).toString(16);
            }

            return uuid;
        },
        pluralize: function (count, word) {
            return count === 1 ? word : word + 's';
        },
        store: function (namespace, data) {
            if (arguments.length > 1) {
                return localStorage.setItem(namespace, JSON.stringify(data));
            } else {
                var store = localStorage.getItem(namespace);
                return (store && JSON.parse(store)) || [];
            }
        }
    };

    var service = {

        init: function(handler) {
            $.get("api/account/register", function(data) {
                handler();
            });
        },

        getAll: function(handler) {
            $.get("api/todos" , function(data) {
                handler(data);
            });
        },

        create: function(todo, handler) {
            $.ajax({
                url: 'api/todos/add', 
                type: 'POST',
                data: JSON.stringify(todo),
                processData: false,
                contentType: "application/json"})
                .done(function(data) {
                    handler(data);
                });
        },

        update: function(todo, handler) {
            $.ajax({
                url: 'api/todos/' + todo.id, 
                type: 'PUT',
                data: JSON.stringify(todo),
                processData: false,
                contentType: "application/json"})
                .done(function(data) {
                    handler(data);
                });
        },

        delete: function(id, handler) {
            $.ajax({
                url: 'api/todos/' + id, 
                type: 'DELETE' })
                .done(function(data) {
                    handler(data);
                });
        }

    };

    var App = {
        init: function () {
            var self = this;
            this.todos = util.store('todos-jquery');
            service.init(function() {
                service.getAll(function(output) {
                    self.todos = output;
                    self.todoTemplate = Handlebars.compile($('#todo-template').html());
                    self.footerTemplate = Handlebars.compile($('#footer-template').html());
                    self.bindEvents();
    
                    new Router({
                        '/:filter': function (filter) {
                            self.filter = filter;
                            self.render();
                        }.bind(self)
                    }).init('/all');
                });
            });
        },

        bindEvents: function () {
            $('#new-todo').on('keyup', this.create.bind(this));
            $('#toggle-all').on('change', this.toggleAll.bind(this));
            $('#footer').on('click', '#clear-completed', this.destroyCompleted.bind(this));
            $('#todo-list')
                .on('change', '.toggle', this.toggle.bind(this))
                .on('dblclick', 'label', this.editingMode.bind(this))
                .on('keyup', '.edit', this.editKeyup.bind(this))
                .on('focusout', '.edit', this.update.bind(this))
                .on('click', '.destroy', this.destroy.bind(this));
        },

        render: function () {
            var todos = this.getFilteredTodos();
            $('#todo-list').html(this.todoTemplate(todos));
            $('#main').toggle(todos.length > 0);
            $('#toggle-all').prop('checked', this.getActiveTodos().length === 0);
            this.renderFooter();
            $('#new-todo').focus();
            util.store('todos-jquery', this.todos);
        },

        renderFooter: function () {
            var todoCount = this.todos.length;
            var activeTodoCount = this.getActiveTodos().length;
            var template = this.footerTemplate({
                activeTodoCount: activeTodoCount,
                activeTodoWord: util.pluralize(activeTodoCount, 'item'),
                completedTodos: todoCount - activeTodoCount,
                filter: this.filter
            });

            $('#footer').toggle(todoCount > 0).html(template);
        },

        toggleAll: function (e) {
            var isChecked = $(e.target).prop('checked');

            this.todos.forEach(function (todo) {
                todo.completed = isChecked;
            });

            this.render();
        },

        getActiveTodos: function () {
            return this.todos.filter(function (todo) {
                return !todo.completed;
            });
        },

        getCompletedTodos: function () {
            return this.todos.filter(function (todo) {
                return todo.completed;
            });
        },

        getFilteredTodos: function () {
            if (this.filter === 'active') {
                return this.getActiveTodos();
            }

            if (this.filter === 'completed') {
                return this.getCompletedTodos();
            }

            return this.todos;
        },

        destroyCompleted: function () {
            this.todos = this.getActiveTodos();
            this.filter = 'all';
            this.render();
        },

        // accepts an element from inside the `.item` div and
        // returns the corresponding index in the `todos` array
        getIndexFromEl: function (el) {
            var id = $(el).closest('li').data('id');
            var todos = this.todos;
            var i = todos.length;

            while (i--) {
                if (todos[i].id === id) {
                    return i;
                }
            }
        },

        create: function (e) {

            var self = this;
            var $input = $(e.target);
            var val = $input.val().trim();

            if (e.which !== ENTER_KEY || !val) {
                return;
            }

            var todo = { id: util.uuid(), title: val, completed: false };

            service.create(todo, function(output) {
                self.todos.push(todo);
                $input.val('');
                self.render();
            });
        },

        toggle: function (e) {
            var self = this;
            var i = this.getIndexFromEl(e.target);
            var todo = this.todos[i];
            todo.completed = !todo.completed;

            service.update(todo, function(output) {
                self.todos[i] = todo;
                self.render();
            });
        },

        editingMode: function (e) {
            var $input = $(e.target).closest('li').addClass('editing').find('.edit');
            $input.val($input.val()).focus();
        },

        editKeyup: function (e) {
            if (e.which === ENTER_KEY) {
                e.target.blur();
            }

            if (e.which === ESCAPE_KEY) {
                $(e.target).data('abort', true).blur();
            }
        },

        update: function (e) {
            var self = this;
            var el = e.target;
            var $el = $(el);
            var val = $el.val().trim();

            if (!val) {
                this.destroy(e);
                return;
            }

            if ($el.data('abort')) {
                $el.data('abort', false);
                this.render();
            } else {
                var todo = this.todos[this.getIndexFromEl(el)];
                todo.title = val;
                service.update(todo, function(output) {
                    self.todos[self.getIndexFromEl(el)].title = val;
                    self.render();
                });

            }
        },

        destroy: function (e) {
            var self = this;
            var todo = this.todos[this.getIndexFromEl(e.target)];

            service.delete(todo.id, function(output) {
                self.todos.splice(self.getIndexFromEl(e.target), 1);
                self.render();
            });
        }
    };

    App.init();
});