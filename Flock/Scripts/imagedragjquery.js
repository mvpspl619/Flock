﻿(function (e, t, n) { e.fn.imagedrag = function (t) { function n(e, t) { i(e, t) } function i(t, n) { var i = e("img", r); var s = e(n.input); var o = r.height(); var u = r.width(); var a = i.height(); var f = i.width(); i.width(u); i.height(u / f * a); var l = i.height(); var c = i.width(); var h = (l - o) * -1; i.draggable({ axis: "y", cursor: n.cursor, create: function () { var e = "0px"; if (n.position.toLowerCase() == "top" || n.position == "" || n.position == " " || n.position == null) { } else if (n.position.toLowerCase() == "middle") { e = h / 2 + "px" } else if (n.position.toLowerCase() == "bottom") { e = h + "px" } else { e = n.position } i.css("top", e); if (n.attribute == "html") s.html(e); else s.attr(n.attribute, e) }, drag: function (e, t) { var r = t.position.top; if (r > 0) { t.position.top = 0 } if (r < h) { t.position.top = h } if (n.attribute == "html") s.html(t.position.top + "px"); else s.attr(n.attribute, t.position.top + "px") } }) } var r; var s = { input: "#output", attribute: "value", position: "middle", cursor: "move" }; var o = e.extend(s, t); return this.each(function () { r = e(this); if (r == null) return false; n(r, o) }) } })(jQuery, this)