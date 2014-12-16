(function ($) {

    $(function () {

        $('.rule-row').click(function (e) {
            /// <param name="e" type="jQuery.Event">Description</param>
            var $currentTarget = $(e.currentTarget),
                itemId = $currentTarget.data('itemid'),
                showSelector = '.condition[data-itemid="' + itemId + '"]';

            if ($currentTarget.hasClass('condition-hidden')) {
                $(showSelector).removeClass('hide');
                $currentTarget.removeClass('condition-hidden')
                    .addClass('condition-shown');
            } else {
                $(showSelector).addClass('hide');
                $currentTarget.addClass('condition-hidden')
                    .removeClass('condition-shown');
            }

        });

    });

})(window.jQuery);