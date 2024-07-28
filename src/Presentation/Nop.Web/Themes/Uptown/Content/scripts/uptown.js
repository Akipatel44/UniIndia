(function ($, ssCore, ssEx) {

    window.themeSettings = {
        themeBreakpoint: 1024,
        isAccordionMenu: false
    };

    var imageUrlAttribute = "href";

    function removeInlineStyle(element) {
        element.removeAttr("style");
    }

    function mobileNavigationClickCallback (e) {
        var nextDropdown = $(this).next();
        $('#header-links-opener .header-selectors-wrapper, #account-links .header-links-wrapper, .responsive-nav-wrapper .search-wrap .search-box, .flyout-cart').not(nextDropdown).slideUp();
        e.stopPropagation();
        nextDropdown.slideToggle('slow');
    }

    function handleSearchBox() {

        $(document).on('themeBreakpointPassed7Spikes', function (e) {

            var searchBox = $('.store-search-box');
            removeInlineStyle(searchBox);

            if (e.isMobileResolution) {
                // mobile
                searchBox.detach().insertAfter('.responsive-nav-wrapper .search-wrap span');
            }
            else if (e.isInitialLoad == false) {
                // desktop
                searchBox.detach().appendTo('.header-cart-search-wrapper');
            }
        });

        $('.responsive-nav-wrapper .search-wrap span').on('click', mobileNavigationClickCallback);

            /* temp fix after mobile search opener behaviour was changed in the core script.
               to be removed or updated once the core script is refactored */

            $('.responsive-nav-wrapper .search-wrap span').on('click', function () {
                $('.overlayOffCanvas').addClass('hidden');
                $('html, body').addClass('scrollable');
            });
            $('.responsive-nav-wrapper .menu-title span').on('click', function () {
                $('.overlayOffCanvas').removeClass('hidden');
                $('html, body').removeClass('scrollable');
            });

            /* end of the temp fix */
    }

    function handleHeaderSelectors() {

        $(document).on('themeBreakpointPassed7Spikes', function (e) {

            var headerSelectorsWrapper = $('.header-selectors-wrapper');
            removeInlineStyle(headerSelectorsWrapper);

            if (e.isMobileResolution) {
                // mobile
                headerSelectorsWrapper.detach().insertAfter('.responsive-nav-wrapper .personal-button span');
            }
            else if (e.isInitialLoad == false) {
                // desktop
                headerSelectorsWrapper.detach().appendTo('.header-links-selectors-wrapper');
            }
        });

        $('#header-links-opener span').on('click', mobileNavigationClickCallback)
    }

    function handleHeaderAccountLinks() {

        $(document).on('themeBreakpointPassed7Spikes', function (e) {

            var headerLinksWrapper = $('.header-links-wrapper');
            removeInlineStyle(headerLinksWrapper);

            if (e.isMobileResolution) {
                // mobile
                headerLinksWrapper.detach().insertAfter('.responsive-nav-wrapper .account-links span');
            }
            else if (e.isInitialLoad == false) {
                // desktop
                headerLinksWrapper.detach().prependTo('.header-links-selectors-wrapper');
            }
        });

        $('#account-links span').on('click', mobileNavigationClickCallback)
    }

    function handleShoppingCart() {

        $(document).on('themeBreakpointPassed7Spikes', function (e) {

            var cartWrapper = $('.cart-wrapper');
            removeInlineStyle($('.flyout-cart'));

            if (e.isMobileResolution) {
                // mobile
                cartWrapper.detach().insertAfter('.search-wrap');
            }
            else if (e.isInitialLoad == false) {
                // desktop
                cartWrapper.detach().prependTo('.header-cart-search-wrapper');
            }
        });

        $('.responsive-nav-wrapper').on('click', '#topcartlink', function (e) {
            if (ssCore.getViewPort().width <= window.themeSettings.themeBreakpoint) {
                var nextDropdown = $(this).next();
                $('#header-links-opener .header-selectors-wrapper, #account-links .header-links-wrapper, .responsive-nav-wrapper .search-wrap .search-box, .flyout-cart').not(nextDropdown).slideUp();
                e.stopPropagation();
                nextDropdown.slideToggle('slow');
            }
        });

        $('.responsive-nav-wrapper').on('click', '.ico-cart', function (e) {
            if (ssCore.getViewPort().width <= window.themeSettings.themeBreakpoint) {
                e.preventDefault();
            }
        });
    }

    function handleNewsletterSubscribeDetach() {

        $(document).on('themeBreakpointPassed7Spikes', function (e) {
            if (e.isMobileResolution) {
                // mobile
                $('.newsletter-popup-overlay .newsletter').insertAfter('.newsletter-box-description');
            }
            else if (e.isInitialLoad == false) {
                // desktop
                $('.footer-1 .newsletter').insertAfter('.newsletter-popup-description');
            }
        });

    }

    function handleFooterChanges() {

        $(document).on('themeBreakpointPassed7Spikes', function (e) {
            if (e.isMobileResolution) {
                // mobile
                $('.footer .social-sharing').detach().insertAfter('.footer .footer-about-us');
            }
            else if (e.isInitialLoad == false) {
                // desktop
                $('.footer .social-sharing').detach().insertAfter('.footer .footer-block.first .footer-menu');
            }
        });

        $(document).on('themeBreakpointPassed7Spikes', function (e) {
            if (e.isMobileResolution) {
                // mobile
                $('.footer-2 .newsletter').detach().insertAfter('.footer-2 .footer-block.last .footer-menu');
            }
            else if (e.isInitialLoad == false) {
                // desktop
                $('.footer-2 .newsletter').detach().insertAfter('.footer-2 .footer-block.first .footer-about-us');
            }
        });
    }

    function handleFooterBlocksCollapse() {
        $(".footer-block .title").click(function (e) {
            if (ssCore.getViewPort().width <= window.themeSettings.themeBreakpoint) {
                $(this).siblings(".footer-collapse").slideToggle("slow");
            }
            else {
                e.preventdefault();
                $(this).siblings(".footer-collapse").show();
            }
        });
    }

    function handleOneColumnFiltersCollapse() {
        $(".uptown-one-column-ajax-filters-wrapper .filtersTitlePanel").click(function (e) {
            if (ssCore.getViewPort().width > window.themeSettings.themeBreakpoint) {
                $(this).siblings(".filtersPanel").slideToggle("slow");
            }
        });
    }

    function incrementQuantityValue(event) {
        event.preventDefault();
        event.stopPropagation();

        var input = $(this).siblings('.qty-input, .productQuantityTextBox').first();

        var value = parseInt(input.val());
        if (isNaN(value)) {
            input.val(1);
            return;
        }

        value++;
        input.val(value);
    }

    function decrementQuantityValue(event) {
        event.preventDefault();
        event.stopPropagation();

        var input = $(this).siblings('.qty-input, .productQuantityTextBox').first();

        var value = parseInt(input.val());

        if (isNaN(value)) {
            input.val(1);
            return;
        }

        if (value <= 1) {
            return;
        }

        value--;
        input.val(value);
    }

    function handleProductPageThumbs(gallerySelector) {
        var isInCarousel = $(gallerySelector).find('.picture-thumbs').hasClass('in-carousel');
        var thumbInfoSelector = $(gallerySelector).find('.cloudzoom-gallery, .thumb-popup-link, .thumb-item');
        var thumbParentSelector = $(gallerySelector).find('.gallery, .sale-item');
        var galleries = $(thumbParentSelector);

        var navigationArrowClickHandler = function () {
            var that = $(this);
            var fullSizeImageUrl = that.find('img').attr('data-fullSizeImageUrl');

            that.trigger('mouseenter');

            var correspondingThumb = that.closest('.product-essential, .product-element').find(thumbParentSelector).find(thumbInfoSelector).filter('[' + imageUrlAttribute + '="' + fullSizeImageUrl + '"]').filter(function () {
                return !$(this).parent().hasClass('slick-cloned');
            });

            if (correspondingThumb.hasClass('cloudzoom-gallery')) {

                var productCloudZoomInstance = $('#cloudZoomImage').data('CloudZoom');
                if (productCloudZoomInstance) {

                    productCloudZoomInstance.closeZoom();
                }

                var quickViewCloudZoomInstance = $('#quick-view-zoom').data('CloudZoom');
                if (quickViewCloudZoomInstance) {

                    quickViewCloudZoomInstance.closeZoom();
                }

                correspondingThumb.click();
            } else {
                $.event.trigger({
                    type: 'nopMainProductImageChanged',
                    target: that,
                    pictureDefaultSizeUrl: fullSizeImageUrl,
                    pictureFullSizeUrl: fullSizeImageUrl
                });
            }

            if (isInCarousel) {

                $('#picture-thumbs-carousel').slick('slick' + $(this).attr('data-type'));
            }
        };

        var setPreviousNavigationImage = function (thumbInfoParent) {
            var thumbInfos = thumbInfoParent.find(thumbInfoSelector).filter(function () {
                return !$(this).parent().hasClass('slick-cloned');
            });

            var activeThumbInfo = thumbInfos.filter('.active');
            var target = activeThumbInfo.prev(thumbInfoSelector);

            if (isInCarousel) {

                target = activeThumbInfo.parent('.picture-thumbs-item').prev('.picture-thumbs-item:not(.slick-cloned)').find(thumbInfoSelector);
            }

            if (target.length === 0) {
                target = thumbInfos.last();
            }

            $(thumbInfoParent).find('.picture-thumbs-prev-arrow img').attr('src', target.find('img').attr('src')).attr('data-fullSizeImageUrl', target.attr(imageUrlAttribute))
                .attr('alt', target.find('img').attr('alt')).attr('title', target.attr('title'));
        };

        var setNextNavigationImage = function (thumbInfoParent) {
            var thumbInfos = thumbInfoParent.find(thumbInfoSelector).filter(function () {
                return !$(this).parent().hasClass('slick-cloned');
            });

            var activeThumbInfo = thumbInfos.filter('.active');
            var target = activeThumbInfo.next(thumbInfoSelector);

            if (isInCarousel) {

                target = activeThumbInfo.parent('.picture-thumbs-item').next('.picture-thumbs-item:not(.slick-cloned)').find(thumbInfoSelector);
            }

            if (target.length === 0) {
                target = thumbInfos.first();
            }

            $(thumbInfoParent).find('.picture-thumbs-next-arrow img').attr('src', target.find('img').attr('src')).attr('data-fullSizeImageUrl', target.attr(imageUrlAttribute))
                .attr('alt', target.find('img').attr('alt')).attr('title', target.attr('title'));
        };

        galleries.each(function () {
            var thisParent = $(this);
            var thumbInfos = thisParent.find(thumbInfoSelector).filter(function () {
                return !$(this).parent().hasClass('slick-cloned');
            });

            thumbInfos.first().addClass('active');
        });

        // Previous arrow
        galleries.find('.picture-thumbs-prev-arrow').on('mouseenter', function () {
            setPreviousNavigationImage($(this).closest(thumbParentSelector));

            $(this).addClass('hovered');
        }).on('mouseleave', function () {
            $(this).removeClass('hovered');
        }).on('click', navigationArrowClickHandler);

        // Next arrow
        galleries.find('.picture-thumbs-next-arrow').on('mouseenter', function () {
            setNextNavigationImage($(this).closest(thumbParentSelector));

            $(this).addClass('hovered');
        }).on('mouseleave', function () {
            $(this).removeClass('hovered');
        }).on('click', navigationArrowClickHandler);

        // Product Thumbs
        $('.gallery .thumb-popup-link, #sevenspikes-cloud-zoom .cloudzoom-gallery, .gallery .cloudzoom-gallery').on('click', function (e) {
            var that = $(this);
            var fullImageUrl = that.attr(imageUrlAttribute);

            //this is needed .thumb-popup-link. We need to prevent the default behaviour of the click event otherwise the picture url will be opened
            // because the thimbs are anchor tags.
            e.preventDefault();

            if (that.hasClass('cloudzoom-gallery')) {

                var productCloudZoomInstance = $('#cloudZoomImage').data('CloudZoom');
                if (productCloudZoomInstance) {
                    
                    productCloudZoomInstance.closeZoom();
                }
                
                var quickViewCloudZoomInstance = $('#quick-view-zoom').data('CloudZoom');
                if (quickViewCloudZoomInstance) {

                    quickViewCloudZoomInstance.closeZoom();
                }
            }

            $.event.trigger({
                type: 'nopMainProductImageChanged',
                target: that,
                pictureDefaultSizeUrl: fullImageUrl,
                pictureFullSizeUrl: fullImageUrl
            });
        });

        $(document).on('nopMainProductImageChanged', function (e) {
            var thumbInfoParent = $(e.target).closest('.product-essential, .product-element').find(thumbParentSelector);
            var correspondingThumb = thumbInfoParent.find(thumbInfoSelector).filter('[' + imageUrlAttribute + '="' + e.pictureFullSizeUrl + '"]').filter(function () {
                return !$(this).parent().hasClass('slick-cloned');
            });

            if (correspondingThumb.length === 0) {
                return;
            }

            // This is needed when the cloud zoom is disabled to change the pictures. If the cloud zoom is enabled this is handled by it.
            thumbInfoParent.find('#zoom1, .picture a[id^="main-product-img-lightbox-anchor"]').attr(imageUrlAttribute, e.pictureFullSizeUrl);
            thumbInfoParent.find('#cloudZoomImage, .picture img[id^="main-product-img"], .item-picture > a > img, .picture > img').attr('src', e.pictureDefaultSizeUrl);

            correspondingThumb.addClass('active');

            var siblings = correspondingThumb.siblings(thumbInfoSelector);

            if (isInCarousel) {
                
                siblings = correspondingThumb.parent('.picture-thumbs-item').siblings('.picture-thumbs-item:not(.slick-cloned)').find(thumbInfoSelector);
            }

            siblings.removeClass('active');

            setPreviousNavigationImage(thumbInfoParent);
            setNextNavigationImage(thumbInfoParent);
        });
    }

    function handleRemovePrevNextTitle() {
        $('.previous-product a, .next-product a').removeAttr("title");
    }

    function handleFlyoutCartScrolling(isInitialLoad) {
        if (isInitialLoad) {
            $('.cart-wrapper .flyout-cart').css({ 'opacity': '0', 'display': 'block' });
        }

        var windowHeight = ssCore.getViewPort().height;
        var miniShoppingCart = $('.mini-shopping-cart');
        var miniShoppingCartItems = $('.mini-shopping-cart').children('.items');
        var responsiveNavWrapper = $('.responsive-nav-wrapper.stick');
        var miniShoppingCartOffsetTop = miniShoppingCart.offset().top;

        if (responsiveNavWrapper.length > 0) {
            miniShoppingCartOffsetTop = miniShoppingCart.position().top + $('.flyout-cart').position().top;
        }
        
        var miniShoppingCartHeight = miniShoppingCart.outerHeight();
        var miniShoppingCartItemsHeight = miniShoppingCartItems.outerHeight();
        var newItemsHeight = (windowHeight - miniShoppingCartOffsetTop - (miniShoppingCartHeight - miniShoppingCartItemsHeight) - 10);

        miniShoppingCartItems.css('max-height', newItemsHeight + 'px');
        miniShoppingCartItems.perfectScrollbar({
            swipePropagation: false,
            wheelSpeed: 1,
            suppressScrollX: true
        });

        if (isInitialLoad) {
            $('.cart-wrapper .flyout-cart').css({ 'display': '', 'opacity': '' });
        }
    }

    function handleFlyoutCartScroll() {
        handleFlyoutCartScrolling(true);

        $(window).on('resize orientationchange', function () {
            setTimeout(function () {
                handleFlyoutCartScrolling(true);
            }, 200);
        });

        $('.header-cart-search-wrapper').on('mouseenter', '.cart-wrapper', function () {
            if (ssCore.getViewPort().width > window.themeSettings.themeBreakpoint) {
                setTimeout(handleFlyoutCartScrolling, 200);
            }
        });

        $('.responsive-nav-wrapper').on('click', '.ico-cart', function () {
            setTimeout(handleFlyoutCartScrolling, 800);
        });
    }

    function handleRemoveItemFromFlyoutCart() {
        $('body').on('click', '.mini-shopping-cart-item-close', function (e) {
            e.preventDefault();

            var flyoutShoppingCartPanelSelector = '#flyout-cart';
            var flyoutShoppingCart = $(flyoutShoppingCartPanelSelector);
            var shoppingCartId = parseInt($(this).closest('.item').attr('data-shoppingCartId'));

            if (isNaN(shoppingCartId) || shoppingCartId === 0) {
                return;
            }

            $.ajax({
                cache: false,
                type: 'POST',
                url: flyoutShoppingCart.attr('data-removeItemFromCartUrl'),
                data: {
                    'shoppingCartId': shoppingCartId
                }
            }).done(function (data) {
                if (data.success) {
                    $.ajax({
                        cache: false,
                        type: 'GET',
                        url: flyoutShoppingCart.attr('data-flyoutCartUrl')
                    }).done(function (data) {
                        var newFlyoutShoppingCart = $(data).filter(flyoutShoppingCartPanelSelector);
                        flyoutShoppingCart.replaceWith(newFlyoutShoppingCart);

                        $('#flyout-cart').trigger('mouseenter');

                        $(document).trigger({ type: 'removeItemFlyoutCart', shoppingCartId: shoppingCartId });
                    });
                }
            });
        });
    }

    function handleClearCartButton() {
        $('.order-summary-content .clear-cart-button').on('click', function (e) {
            e.preventDefault();

            var productsInShoppingCart = $('.cart tbody tr');

            for (var i = 0; i < productsInShoppingCart.length; i++)
            {
                var product = productsInShoppingCart[i];

                var canBeRemovedFromCart = $(product).find('.remove-from-cart input').length > 0;

                if (!canBeRemovedFromCart) {
                    var qtyInputName = $(product).find('.qty-input').attr('name');

                    var shoppingCartItemId = qtyInputName.replace('itemquantity', '');

                    $(product).find('.remove-from-cart').append('<input type="checkbox" name="removefromcart" value="' + shoppingCartItemId + '" style="display:none;">');
                }
            }

            $('.cart [name="removefromcart"]').attr('checked', 'checked');

            $('.order-summary-content .update-cart-button').click();
        });
    }
    
    function initializePlusAndMinusQuantityButtonsClick() {
        $("body").on("click", ".add-to-cart-qty-wrapper .plus", incrementQuantityValue)
            .on("click", ".add-to-cart-qty-wrapper .minus", decrementQuantityValue)
            .on("click", ".ajax-cart-button-wrapper .plus", incrementQuantityValue)
            .on("click", ".ajax-cart-button-wrapper .minus", decrementQuantityValue);
    }

    $(document).ready(function () {
        if ($('#sevenspikes-cloud-zoom').length > 0) {
            imageUrlAttribute = "data-full-image-url";
        } 

        var menuDesktopStickElement = ".header-lower .header-centering";
        var menuDesktopStickParentElement = ".header-lower";

        if ($('.header-2').length > 0) {
            menuDesktopStickElement = "#headerMenuParent";
            menuDesktopStickParentElement = ".header-menu-wrapper";
        }

        var responsiveAppSettings = {
            themeBreakpoint: window.themeSettings.themeBreakpoint,
            isEnabled: true,
            // currently we do not use the built in functionality fot attaching and detaching, because we want to avoid the overlays canvas being displayed
            isSearchBoxDetachable: false,
            isHeaderLinksWrapperDetachable: false,
            doesDesktopHeaderMenuStick: true,
            doesScrollAfterFiltration: true,
            doesSublistHasIndent: true,
            displayGoToTop: true,
            hasStickyNav: true,
            selectors: {
                menuTitle: ".menu-title",
                headerMenu: ".header-menu",
                closeMenu: ".close-menu",
                movedElements: ".admin-header-links, .header-upper, .breadcrumb, .header-logo, .responsive-nav-wrapper, .slider-wrapper, .master-column-wrapper, .footer",
                sublist: ".header-menu .sublist",
                overlayOffCanvas: ".overlayOffCanvas",
                withSubcategories: ".with-subcategories",
                filtersContainer: ".nopAjaxFilters7Spikes",
                filtersOpener: ".filters-button span",
                searchBoxOpener: ".search-wrap > span",
                // currently we do not use the built in functionality fot attaching and detaching, because we want to avoid the overlays canvas being displayed
                searchBox: ".search-box",
                searchBoxBefore: ".header-cart-search-wrapper .cart-wrapper",
                navWrapper: ".responsive-nav-wrapper",
                navWrapperParent: ".responsive-nav-wrapper-parent",
                headerMenuDesktopStickElement: menuDesktopStickElement,
                headerMenuDesktopStickParentElement: menuDesktopStickParentElement,
                headerLinksOpener: "",
                headerLinksWrapper: ".header-links-selectors-wrapper",
                headerLinksWrapperMobileInsertAfter: ".header",
                headerLinksWrapperDesktopPrependTo: ".header-upper-centering",
                shoppingCartLink: "",
                overlayEffectDelay: 300
            }
        };

        handleSearchBox();
        handleHeaderSelectors();
        handleHeaderAccountLinks();
        handleShoppingCart();
        handleNewsletterSubscribeDetach();
        handleFooterChanges();

        ssEx.initResponsiveTheme(responsiveAppSettings);

        handleProductPageThumbs('.product-details-page');
        handleRemovePrevNextTitle();
        handleFooterBlocksCollapse();
        handleOneColumnFiltersCollapse();
        handleRemoveItemFromFlyoutCart();
        handleClearCartButton();
        initializePlusAndMinusQuantityButtonsClick();

        if ($('#isShoppingCartEnabled').val() === 'true') {
            handleFlyoutCartScroll();
        }

        $('.newsletter-subscribe-block-opener').on('click', function(e) {
            e.preventDefault();

            var newsletterEmail = $('#newsletter-subscribe-block');

            if (newsletterEmail.is(':visible') && newsletterEmail.find('#newsletter-email').val() !== '') {
                $('#newsletter-subscribe-button').click();
                return;
            }

            var newsletterPopup = $('.newsletter-subscribe-popup-overlay');

            if (newsletterPopup.length > 0) {

                newsletterEmail.show();

            } else {

                newsletterEmail.slideToggle();
            }
        });

        $('body').on('click', function (e) {
            if (ssCore.getViewPort().width <= window.themeSettings.themeBreakpoint) {
                if ($(e.target).parents(".responsive-nav-wrapper").length == 0) {
                    $('#header-links-opener .header-selectors-wrapper, #account-links .header-links-wrapper, .responsive-nav-wrapper .search-wrap .search-box, .flyout-cart').slideUp();
                }
            }
        });

        $(document).on("nopQuickViewDataShownEvent", function () {
            handleProductPageThumbs('.quickView');
        });
    });

    $(window).on('load', function () {
        $('.loader-overlay').hide();
    });

})(jQuery, sevenSpikesCore, sevenSpikesEx);