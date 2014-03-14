flockApp.factory('quacksFactory', function ($http) {
    var quacksFactory = function () {
        this.items = [];
        this.busy = false;
        this.after = 15;
    };

    quacksFactory.prototype.nextQuacks = function () {
        if (this.busy) return;
        this.busy = true;

        var url = "http://localhost:55886/api/quack/activeQuacks?quackCount=" + this.after;
        $http.jsonp(url).success(function (data) {
            var items = data;
            console.log(items);
            for (var i = 0; i < data.length; i++) {
                this.items.push(items[i]);
            }
            this.after = this.items.count;
            this.busy = false;
        }.bind(this));
    };
    return quacksFactory;
});