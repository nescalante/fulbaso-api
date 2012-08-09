/*
* jQuery UI selectmenu
*
* Copyright (c) 2009 AUTHORS.txt (http://jqueryui.com/about)
* Dual licensed under the MIT (MIT-LICENSE.txt)
* and GPL (GPL-LICENSE.txt) licenses.
*
* http://docs.jquery.com/UI
*/

(function ($) {

    $.widget("ui.selectmenu", {
        _init: function () {
            var self = this, o = this.options;

            //quick array of button and menu id's
            this.ids = [this.element.attr('id') + '-' + 'button', this.element.attr('id') + '-' + 'menu'];

            //define safe mouseup for future toggling
            this._safemouseup = true;

            //create menu button wrapper
            this.newelement = $('<a class="' + this.widgetBaseClass + ' ui-widget ui-widget-content ui-corner-all" id="' + this.ids[0] + '" role="button" href="#" aria-haspopup="true" aria-owns="' + this.ids[1] + '"></a>')
			.insertAfter(this.element);

            //transfer tabindex
            var tabindex = this.element.attr('tabindex');
            if (tabindex) { this.newelement.attr('tabindex', tabindex); }

            //save reference to select in data for ease in calling methods
            this.newelement.data('selectelement', this.element);

            //menu icon
            this.selectmenuIcon = $('<span class="' + this.widgetBaseClass + '-icon ui-icon"></span>')
			.prependTo(this.newelement)
			.addClass((o.style == "popup") ? 'ui-icon-triangle-2-n-s' : 'ui-icon-triangle-1-s');


            //make associated form label trigger focus
            $('label[for=' + this.element.attr('id') + ']')
			.attr('for', this.ids[0])
			.bind('click', function () {
			    self.newelement[0].focus();
			    return false;
			});

            //click toggle for menu visibility
            this.newelement
			.bind('mousedown', function (event) {
			    self._toggle(event);
			    //make sure a click won't open/close instantly
			    if (o.style == "popup") {
			        self._safemouseup = false;
			        setTimeout(function () { self._safemouseup = true; }, 300);
			    }
			    return false;
			})
			.bind('click', function () {
			    return false;
			})
			.keydown(function (event) {
			    var ret = true;
			    switch (event.keyCode) {
			        case $.ui.keyCode.ENTER:
			            ret = true;
			            break;
			        case $.ui.keyCode.SPACE:
			            ret = false;
			            self._toggle(event);
			            break;
			        case $.ui.keyCode.UP:
			        case $.ui.keyCode.LEFT:
			            ret = false;
			            self._moveSelection(-1);
			            break;
			        case $.ui.keyCode.DOWN:
			        case $.ui.keyCode.RIGHT:
			            ret = false;
			            self._moveSelection(1);
			            break;
			        case $.ui.keyCode.TAB:
			            ret = true;
			            break;
			        default:
			            ret = false;
			            self._typeAhead(event.keyCode, 'mouseup');
			            break;
			    }
			    return ret;
			})
			.bind('mouseover focus', function () {
			    $(this).addClass(self.widgetBaseClass + '-focus ui-state-hover');
			})
			.bind('mouseout blur', function () {
			    $(this).removeClass(self.widgetBaseClass + '-focus ui-state-hover');
			});

            //document click closes menu
            $(document)
			.mousedown(function (event) {
			    self.close(event);
			});

            //change event on original selectmenu
            this.element
			.click(function () { this._refreshValue(); })
			.focus(function () { this.newelement[0].focus(); });

            //create menu portion, append to body
            var cornerClass = (o.style == "dropdown") ? " ui-corner-bottom" : " ui-corner-all"
            this.list = $('<ul class="' + self.widgetBaseClass + '-menu ui-widget ui-widget-content' + cornerClass + '" aria-hidden="true" role="listbox" aria-labelledby="' + this.ids[0] + '" id="' + this.ids[1] + '"></ul>').appendTo('body');

            //serialize selectmenu element options	
            var selectOptionData = [];
            this.element
			.find('option')
			.each(function () {
			    selectOptionData.push({
			        value: $(this).attr('value'),
			        text: self._formatText(jQuery(this).text()),
			        selected: $(this).attr('selected'),
			        classes: $(this).attr('class'),
			        parentOptGroup: $(this).parent('optgroup').attr('label')
			    });
			});

            //active state class is only used in popup style
            var activeClass = (self.options.style == "popup") ? " ui-state-active" : "";

            //write li's
            for (var i in selectOptionData) {
                var thisLi = $('<li role="presentation"><a href="#" tabindex="-1" role="option" aria-selected="false">' + selectOptionData[i].text + '</a></li>')
				.data('index', i)
				.addClass(selectOptionData[i].classes)
				.data('optionClasses', selectOptionData[i].classes || '')
				.mouseup(function (event) {
				    if (self._safemouseup) {
				        var changed = $(this).data('index') != self._selectedIndex();
				        self.value($(this).data('index'));
				        self.select(event);
				        if (changed) { self.change(event); }
				        self.close(event, true);
				    }
				    return false;
				})
				.click(function () {
				    return false;
				})
				.bind('mouseover focus', function () {
				    self._selectedOptionLi().addClass(activeClass);
				    self._focusedOptionLi().removeClass(self.widgetBaseClass + '-item-focus ui-state-hover');
				    $(this).removeClass('ui-state-active').addClass(self.widgetBaseClass + '-item-focus ui-state-hover');
				})
				.bind('mouseout blur', function () {
				    if ($(this).is(self._selectedOptionLi())) { $(this).addClass(activeClass); }
				    $(this).removeClass(self.widgetBaseClass + '-item-focus ui-state-hover');
				});

                //optgroup or not...
                if (selectOptionData[i].parentOptGroup) {
                    var optGroupName = self.widgetBaseClass + '-group-' + selectOptionData[i].parentOptGroup.split(' ').join('');
                    if (this.list.find('li.' + optGroupName).size()) {
                        this.list.find('li.' + optGroupName + ':last ul').append(thisLi);
                    }
                    else {
                        $('<li role="presentation" class="' + self.widgetBaseClass + '-group ' + optGroupName + '"><span class="' + self.widgetBaseClass + '-group-label">' + selectOptionData[i].parentOptGroup + '</span><ul></ul></li>')
						.appendTo(this.list)
						.find('ul')
						.append(thisLi);
                    }
                }
                else {
                    thisLi.appendTo(this.list);
                }

                //this allows for using the scrollbar in an overflowed list
                this.list.bind('mousedown mouseup', function () { return false; });

                //append icon if option is specified
                if (o.icons) {
                    for (var j in o.icons) {
                        if (thisLi.is(o.icons[j].find)) {
                            thisLi
							.data('optionClasses', selectOptionData[i].classes + ' ' + self.widgetBaseClass + '-hasIcon')
							.addClass(self.widgetBaseClass + '-hasIcon');
                            var iconClass = o.icons[j].icon || "";

                            thisLi
							.find('a:eq(0)')
							.prepend('<span class="' + self.widgetBaseClass + '-item-icon ui-icon ' + iconClass + '"></span>');
                        }
                    }
                }
            }

            //add corners to top and bottom menu items
            this.list.find('li:last').addClass("ui-corner-bottom");
            if (o.style == 'popup') { this.list.find('li:first').addClass("ui-corner-top"); }

            //transfer classes to selectmenu and list
            if (o.transferClasses) {
                var transferClasses = this.element.attr('class') || '';
                this.newelement.add(this.list).addClass(transferClasses);
            }

            //original selectmenu width
            var clonedSelect = this.element.clone().appendTo('body');
            var selectWidth = clonedSelect.css({ position: "absolute", visibility: "hidden", display: "block" }).width();
            clonedSelect.remove();

            //set menu button width
            this.newelement.width((o.width) ? o.width : selectWidth);

            //set menu width to either menuWidth option value, width option value, or select width 
            if (o.style == 'dropdown') { this.list.width((o.menuWidth) ? o.menuWidth : ((o.width) ? o.width : selectWidth)); }
            else { this.list.width((o.menuWidth) ? o.menuWidth : ((o.width) ? o.width - o.handleWidth : selectWidth - o.handleWidth)); }

            //set max height from option 
            if (o.maxHeight && o.maxHeight < this.list.height()) { this.list.height(o.maxHeight); }

            //save reference to actionable li's (not group label li's)
            this._optionLis = this.list.find('li:not(.' + self.widgetBaseClass + '-group)');

            //transfer menu click to menu button
            this.list
			.keydown(function (event) {
			    var ret = true;
			    switch (event.keyCode) {
			        case $.ui.keyCode.UP:
			        case $.ui.keyCode.LEFT:
			            ret = false;
			            self._moveFocus(-1);
			            break;
			        case $.ui.keyCode.DOWN:
			        case $.ui.keyCode.RIGHT:
			            ret = false;
			            self._moveFocus(1);
			            break;
			        case $.ui.keyCode.HOME:
			            ret = false;
			            self._moveFocus(':first');
			            break;
			        case $.ui.keyCode.PAGE_UP:
			            ret = false;
			            self._scrollPage('up');
			            break;
			        case $.ui.keyCode.PAGE_DOWN:
			            ret = false;
			            self._scrollPage('down');
			            break;
			        case $.ui.keyCode.END:
			            ret = false;
			            self._moveFocus(':last');
			            break;
			        case $.ui.keyCode.ENTER:
			        case $.ui.keyCode.SPACE:
			            ret = false;
			            self.close(event, true);
			            $(event.target).parents('li:eq(0)').trigger('mouseup');
			            break;
			        case $.ui.keyCode.TAB:
			            ret = true;
			            self.close(event, true);
			            break;
			        case $.ui.keyCode.ESCAPE:
			            ret = false;
			            self.close(event, true);
			            break;
			        default:
			            ret = false;
			            self._typeAhead(event.keyCode, 'focus');
			            break;
			    }
			    return ret;
			});

            //selectmenu style
            if (o.style == 'dropdown') {
                this.newelement
				.addClass(self.widgetBaseClass + "-dropdown");
                this.list
				.addClass(self.widgetBaseClass + "-menu-dropdown");
            }
            else {
                this.newelement
				.addClass(self.widgetBaseClass + "-popup");
                this.list
				.addClass(self.widgetBaseClass + "-menu-popup");
            }

            //append status span to button
            this.newelement.prepend('<span class="' + self.widgetBaseClass + '-status">' + selectOptionData[this._selectedIndex()].text + '</span>');

            //hide original selectmenu element
            this.element.hide();

            //transfer disabled state
            if (this.element.attr('disabled') == true) { this.disable(); }

            //update value
            this.value(this._selectedIndex());
        },
        destroy: function () {
            this.element.removeData(this.widgetName)
			.removeClass(this.widgetBaseClass + '-disabled' + ' ' + this.namespace + '-state-disabled')
			.removeAttr('aria-disabled');

            //unbind click on label, reset its for attr
            $('label[for=' + this.newelement.attr('id') + ']')
			.attr('for', this.element.attr('id'))
			.unbind('click');
            this.newelement.remove();
            this.list.remove();
            this.element.show();
        },
        _typeAhead: function (code, eventType) {
            var self = this;
            //define self._prevChar if needed
            if (!self._prevChar) { self._prevChar = ['', 0]; }
            var C = String.fromCharCode(code);
            c = C.toLowerCase();
            var focusFound = false;
            function focusOpt(elem, ind) {
                focusFound = true;
                $(elem).trigger(eventType);
                self._prevChar[1] = ind;
            };
            this.list.find('li a').each(function (i) {
                if (!focusFound) {
                    var thisText = $(this).text();
                    if (thisText.indexOf(C) == 0 || thisText.indexOf(c) == 0) {
                        if (self._prevChar[0] == C) {
                            if (self._prevChar[1] < i) { focusOpt(this, i); }
                        }
                        else { focusOpt(this, i); }
                    }
                }
            });
            this._prevChar[0] = C;
        },
        _uiHash: function () {
            return {
                value: this.value()
            };
        },
        open: function (event) {
            var self = this;
            var disabledStatus = this.newelement.attr("aria-disabled");
            if (disabledStatus != 'true') {
                this._refreshPosition();
                this._closeOthers(event);
                this.newelement
				.addClass('ui-state-active');

                this.list
				.appendTo('body')
				.addClass(self.widgetBaseClass + '-open')
				.attr('aria-hidden', false)
				.find('li:not(.' + self.widgetBaseClass + '-group):eq(' + this._selectedIndex() + ') a')[0].focus();
                if (this.options.style == "dropdown") { this.newelement.removeClass('ui-corner-all').addClass('ui-corner-top'); }
                this._refreshPosition();
                this._trigger("open", event, this._uiHash());
            }
        },
        close: function (event, retainFocus) {
            if (this.newelement.is('.ui-state-active')) {
                this.newelement
				.removeClass('ui-state-active');
                this.list
				.attr('aria-hidden', true)
				.removeClass(this.widgetBaseClass + '-open');
                if (this.options.style == "dropdown") { this.newelement.removeClass('ui-corner-top').addClass('ui-corner-all'); }
                if (retainFocus) { this.newelement[0].focus(); }
                this._trigger("close", event, this._uiHash());
            }
        },
        change: function (event) {
            this.element.trigger('change');
            this._trigger("change", event, this._uiHash());
        },
        select: function (event) {
            this._trigger("select", event, this._uiHash());
        },
        _closeOthers: function (event) {
            $('.' + this.widgetBaseClass + '.ui-state-active').not(this.newelement).each(function () {
                $(this).data('selectelement').selectmenu('close', event);
            });
            $('.' + this.widgetBaseClass + '.ui-state-hover').trigger('mouseout');
        },
        _toggle: function (event, retainFocus) {
            if (this.list.is('.' + this.widgetBaseClass + '-open')) { this.close(event, retainFocus); }
            else { this.open(event); }
        },
        _formatText: function (text) {
            return this.options.format ? this.options.format(text) : text;
        },
        _selectedIndex: function () {
            return this.element[0].selectedIndex;
        },
        _selectedOptionLi: function () {
            return this._optionLis.eq(this._selectedIndex());
        },
        _focusedOptionLi: function () {
            return this.list.find('.' + this.widgetBaseClass + '-item-focus');
        },
        _moveSelection: function (amt) {
            var currIndex = parseInt(this._selectedOptionLi().data('index'), 10);
            var newIndex = currIndex + amt;
            return this._optionLis.eq(newIndex).trigger('mouseup');
        },
        _moveFocus: function (amt) {
            if (!isNaN(amt)) {
                var currIndex = parseInt(this._focusedOptionLi().data('index'), 10);
                var newIndex = currIndex + amt;
            }
            else { var newIndex = parseInt(this._optionLis.filter(amt).data('index'), 10); }

            if (newIndex < 0) { newIndex = 0; }
            if (newIndex > this._optionLis.size() - 1) {
                newIndex = this._optionLis.size() - 1;
            }
            var activeID = this.widgetBaseClass + '-item-' + Math.round(Math.random() * 1000);

            this._focusedOptionLi().find('a:eq(0)').attr('id', '');
            this._optionLis.eq(newIndex).find('a:eq(0)').attr('id', activeID)[0].focus();
            this.list.attr('aria-activedescendant', activeID);
        },
        _scrollPage: function (direction) {
            var numPerPage = Math.floor(this.list.outerHeight() / this.list.find('li:first').outerHeight());
            numPerPage = (direction == 'up') ? -numPerPage : numPerPage;
            this._moveFocus(numPerPage);
        },
        _setData: function (key, value) {
            this.options[key] = value;
            if (key == 'disabled') {
                this.close();
                this.element
				.add(this.newelement)
				.add(this.list)
					[value ? 'addClass' : 'removeClass'](
						this.widgetBaseClass + '-disabled' + ' ' +
						this.namespace + '-state-disabled')
					.attr("aria-disabled", value);
            }
        },
        value: function (newValue) {
            if (arguments.length) {
                this.element[0].selectedIndex = newValue;
                this._refreshValue();
                this._refreshPosition();
            }
            return this.element[0].selectedIndex;
        },
        _refreshValue: function () {
            var activeClass = (this.options.style == "popup") ? " ui-state-active" : "";
            var activeID = this.widgetBaseClass + '-item-' + Math.round(Math.random() * 1000);
            //deselect previous
            this.list
			.find('.' + this.widgetBaseClass + '-item-selected')
			.removeClass(this.widgetBaseClass + "-item-selected" + activeClass)
			.find('a')
			.attr('aria-selected', 'false')
			.attr('id', '');
            //select new
            this._selectedOptionLi()
			.addClass(this.widgetBaseClass + "-item-selected" + activeClass)
			.find('a')
			.attr('aria-selected', 'true')
			.attr('id', activeID);

            //toggle any class brought in from option
            var currentOptionClasses = this.newelement.data('optionClasses') ? this.newelement.data('optionClasses') : "";
            var newOptionClasses = this._selectedOptionLi().data('optionClasses') ? this._selectedOptionLi().data('optionClasses') : "";
            this.newelement
			.removeClass(currentOptionClasses)
			.data('optionClasses', newOptionClasses)
			.addClass(newOptionClasses)
			.find('.' + this.widgetBaseClass + '-status')
			.html(
				this._selectedOptionLi()
					.find('a:eq(0)')
					.html()
			);

            this.list.attr('aria-activedescendant', activeID)
        },
        _refreshPosition: function () {
            //set left value
            this.list.css('left', this.newelement.offset().left);

            //set top value
            var menuTop = this.newelement.offset().top;
            var scrolledAmt = this.list[0].scrollTop;
            this.list.find('li:lt(' + this._selectedIndex() + ')').each(function () {
                scrolledAmt -= $(this).outerHeight();
            });

            if (this.newelement.is('.' + this.widgetBaseClass + '-popup')) {
                menuTop += scrolledAmt;
                this.list.css('top', menuTop);
            }
            else {
                menuTop += this.newelement.height();
                this.list.css('top', menuTop);
            }
        }
    });

    $.extend($.ui.selectmenu, {
        getter: "value",
        version: "@VERSION",
        eventPrefix: "selectmenu",
        defaults: {
            transferClasses: true,
            style: 'popup',
            width: null,
            menuWidth: null,
            handleWidth: 26,
            maxHeight: null,
            icons: null,
            format: null
        }
    });

    $.widget("custom.catcomplete", $.ui.autocomplete, {
        _renderMenu: function (ul, items) {
            var self = this,
				currentCategory = "";
            $.each(items, function (index, item) {
                if (item.category != currentCategory) {
                    ul.append("<li class='ui-autocomplete-category'>" + item.category + "</li>");
                    currentCategory = item.category;
                }
                self._renderItem(ul, item);
            });
        },
        _renderItem: function (ul, item) {
            var re = new RegExp("(" + this.term + ")", "i");
            var t = item.label.replace(re, "<span class='match'>$1</span>");
            return $("<li></li>")
                .data("item.autocomplete", item)
                .append("<a>" + t + "</a>")
                .appendTo(ul);
        }
    });

    $.widget("custom.autocompletematch", $.ui.autocomplete, {
        _renderMenu: function (ul, items) {
            var self = this,
				currentCategory = "";
            $.each(items, function (index, item) {
                /*if (item.category != currentCategory) {
                    ul.append("<li class='ui-autocomplete-category'>" + item.category + "</li>");
                    currentCategory = item.category;
                }*/
                self._renderItem(ul, item);
            });
            ul.addClass("typeahead dropdown-menu");
        },
        _renderItem: function (ul, item) {
            var re = new RegExp("(" + this.term + ")", "i");
            var t = item.label.replace(re, "<span class='match'>$1</span>");
            return $("<li></li>")
                .data("item.autocomplete", item)
                .append("<a>" + t + "</a>")
                .appendTo(ul);
        }
    });

    $.widget("custom.textbox", {
        _init: function () {
            this.element.addClass("text ui-widget-content ui-corner-all");
        }
    });

})(jQuery);

jQuery(function (e, i) {
    function j() { var a = e("script:first"), b = a.css("color"), c = false; if (/^rgba/.test(b)) c = true; else try { c = b != a.css("color", "rgba(0, 0, 0, 0.5)").css("color"); a.css("color", b) } catch (d) { } return c } function k(a, b, c) {
        var d = []; a.c && d.push("inset"); typeof b.left != "undefined" && d.push(parseInt(a.left + c * (b.left - a.left), 10) + "px " + parseInt(a.top + c * (b.top - a.top), 10) + "px"); typeof b.blur != "undefined" && d.push(parseInt(a.blur + c * (b.blur - a.blur), 10) + "px"); typeof b.a != "undefined" && d.push(parseInt(a.a + c *
(b.a - a.a), 10) + "px"); if (typeof b.color != "undefined") { var g = "rgb" + (e.support.rgba ? "a" : "") + "(" + parseInt(a.color[0] + c * (b.color[0] - a.color[0]), 10) + "," + parseInt(a.color[1] + c * (b.color[1] - a.color[1]), 10) + "," + parseInt(a.color[2] + c * (b.color[2] - a.color[2]), 10); if (e.support.rgba) g += "," + parseFloat(a.color[3] + c * (b.color[3] - a.color[3])); g += ")"; d.push(g) } return d.join(" ")
    } function h(a) {
        var b, c, d = {}; if (b = /#([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})/.exec(a)) c = [parseInt(b[1], 16), parseInt(b[2], 16), parseInt(b[3],
16), 1]; else if (b = /#([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])/.exec(a)) c = [parseInt(b[1], 16) * 17, parseInt(b[2], 16) * 17, parseInt(b[3], 16) * 17, 1]; else if (b = /rgb\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*\)/.exec(a)) c = [parseInt(b[1], 10), parseInt(b[2], 10), parseInt(b[3], 10), 1]; else if (b = /rgba\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9\.]*)\s*\)/.exec(a)) c = [parseInt(b[1], 10), parseInt(b[2], 10), parseInt(b[3], 10), parseFloat(b[4])]; d = (b = /(-?[0-9]+)(?:px)?\s+(-?[0-9]+)(?:px)?(?:\s+(-?[0-9]+)(?:px)?)?(?:\s+(-?[0-9]+)(?:px)?)?/.exec(a)) ?
{ left: parseInt(b[1], 10), top: parseInt(b[2], 10), blur: b[3] ? parseInt(b[3], 10) : 0, a: b[4] ? parseInt(b[4], 10) : 0} : { left: 0, top: 0, blur: 0, a: 0 }; d.c = /inset/.test(a); d.color = c; return d
    } e.extend(true, e, { support: { rgba: j()} }); var f; e.each(["boxShadow", "MozBoxShadow", "WebkitBoxShadow"], function (a, b) { a = e("html").css(b); if (typeof a == "string" && a != "") { f = b; return false } }); if (f) e.fx.step.boxShadow = function (a) {
        if (!a.init) {
            a.b = h(e(a.elem).get(0).style[f] || e(a.elem).css(f)); a.end = e.extend({}, a.b, h(a.end)); if (a.b.color == i) a.b.color =
a.end.color || [0, 0, 0]; a.init = true
        } a.elem.style[f] = k(a.b, a.end, a.pos)
    }
});

jQuery(function (d) {
    function h(b, d) {
        var a, c, f = {}; if (a = /#([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})/.exec(b)) c = [parseInt(a[1], 16), parseInt(a[2], 16), parseInt(a[3], 16), 1]; else if (a = /#([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])/.exec(b)) c = [parseInt(a[1], 16) * 17, parseInt(a[2], 16) * 17, parseInt(a[3], 16) * 17, 1]; else if (a = /rgb\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*\)/.exec(b)) c = [parseInt(a[1]), parseInt(a[2]), parseInt(a[3]), 1]; else if (a = /rgba\(\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9]{1,3})\s*,\s*([0-9\.]*)\s*\)/.exec(b)) c =
[parseInt(a[1]), parseInt(a[2]), parseInt(a[3]), parseFloat(a[4])]; if (a = /(-*[0-9.]+(?:px|em|pt)?)\s+(-*[0-9.]+(?:px|em|pt)?)\s+(-*[0-9.]+(?:px|em|pt)?)/.exec(b.replace(a[0], ""))) a = a.slice(1).map(function (a) { var b = a.match(/em|pt/); if (b == "em") return parseFloat(a) * parseInt(d); if (b == "pt") return parseInt(a) / 72 * 96; return parseInt(a) }), f = { right: a[0], bottom: a[1], blur: a[2] }; f.color = c; return f
    } d.extend(!0, d, { support: { rgba: function () {
        var b = d("script:first"), e = b.css("color"), a = !1; if (/^rgba/.test(e)) a = !0; else try {
            a =
e != b.css("color", "rgba(0, 0, 0, 0.5)").css("color"), b.css("color", e)
        } catch (c) { } return a
    } ()
    }
    }); d.fx.step.textShadow = function (b) {
        if (!b.init) { var e = d(b.elem).get(0).style.fontSize || d(b.elem).css("fontSize"), a = d(b.elem).get(0).style.textShadow || d(b.elem).css("textShadow"); if (a == "none") a = b.end; b.begin = h(a, e); b.end = d.extend({}, b.begin, h(b.end, e)); b.init = !0 } var e = b.elem.style, a = b.begin, c = b.end, b = b.pos, f = []; typeof c.right != "undefined" && f.push(parseInt(a.right + b * (c.right - a.right)) + "px " + parseInt(a.bottom +
b * (c.bottom - a.bottom)) + "px"); typeof c.blur != "undefined" && f.push(parseInt(a.blur + b * (c.blur - a.blur)) + "px"); if (typeof c.color != "undefined") { var g = "rgb" + (d.support.rgba ? "a" : "") + "(" + parseInt(a.color[0] + b * (c.color[0] - a.color[0])) + "," + parseInt(a.color[1] + b * (c.color[1] - a.color[1])) + "," + parseInt(a.color[2] + b * (c.color[2] - a.color[2])); d.support.rgba && (g += "," + parseFloat(a.color[3] + b * (c.color[3] - a.color[3]))); g += ")"; f.push(g) } a = f.join(" "); e.textShadow = a
    } 
});