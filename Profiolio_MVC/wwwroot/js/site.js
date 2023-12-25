// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
  /**
   * Mobile nav toggle
   */


(function() {
    "use strict";
  
    /**
     * Easy selector helper function
     */
    const select = (el, all = false) => {
      el = el.trim()
      if (all) {
        return [...document.querySelectorAll(el)]
      } else {
        return document.querySelector(el)
      }
    }
  
    /**
     * Easy event listener function
     */
    const on = (type, el, listener, all = false) => {
      let selectEl = select(el, all)
      if (selectEl) {
        if (all) {
          selectEl.forEach(e => e.addEventListener(type, listener))
        } else {
          selectEl.addEventListener(type, listener)
        }
      }
    }
  
    /**
     * Easy on scroll event listener 
     */
    const onscroll = (el, listener) => {
      el.addEventListener('scroll', listener)
    }
  
    /**
     * Navbar links active state on scroll
     */
    // let navbarlinks = select('#navbar .scrollto', true)
    // const navbarlinksActive = () => {
    //   let position = window.scrollY + 200
    //   navbarlinks.forEach(navbarlink => {
    //     if (!navbarlink.hash) return
    //     let section = select(navbarlink.hash)
    //     if (!section) return
    //     if (position >= section.offsetTop && position <= (section.offsetTop + section.offsetHeight)) {
    //       navbarlink.classList.add('active')
    //     } else {
    //       navbarlink.classList.remove('active')
    //     }
    //   })
    // }
    // window.addEventListener('load', navbarlinksActive)
    // onscroll(document, navbarlinksActive)
  

     let navbarlinks = select('#navbar .scrollto', true)
     
     const navbarlinksActive = () => {
      let section = select('.mypage')
      if(!section) return
      var matches = false;
    //   let position = window.scrollY + 200
       navbarlinks.forEach(navbarlink => {
    //     if (!navbarlink.hash) return
        //console.log(navbarlink.classList);
        navbarlink.classList.remove('active');
        if(!matches){
          section.classList.forEach(entry => {
            if(!matches)
              matches = navbarlink.classList.contains(entry);
            else return
          })
          if(matches)
            {navbarlink.classList.add('active')}
        }
          
        
        

    //     if (!section) return
    //     if (position >= section.offsetTop && position <= (section.offsetTop + section.offsetHeight)) {
    //       navbarlink.classList.add('active')
    //     } else {
    //       navbarlink.classList.remove('active')
    //     }
       })
     }
     window.addEventListener('load', navbarlinksActive)


    /**
     * Scrolls to an element with header offset
     */
    const scrollto = (el) => {
      let elementPos = select(el).offsetTop
      window.scrollTo({
        top: elementPos,
        behavior: 'smooth'
      })
    }
  
    // toggleNavMenu = (clas)=>{
    //   let navbarlinks = select('#navbar .scrollto', true)
        
    //     navbarlinks.forEach(navbarlink => {
    //       if (!navbarlink.hash) return
    //       let section = select(navbarlink.hash)
    //       if (!section) return
    //       if(navbarlink.classList.contains('active'))
    //         navbarlink.classList.remove('active')
    //       if(navbarlink.classList.contains(clas))
    //         navbarlink.classList.remove('active')
    //     })
    
    // }

    /**
     * Back to top button
     */
    let backtotop = select('.back-to-top')
    if (backtotop) {
      const toggleBacktotop = () => {
        if (window.scrollY > 100) {
          backtotop.classList.add('active')
        } else {
          backtotop.classList.remove('active')
        }
      }
      window.addEventListener('load', toggleBacktotop)
      onscroll(document, toggleBacktotop)
    }
  
    /**
     * Mobile nav toggle
     */
    on('click', '.mobile-nav-toggle', function(e) {
      select('body').classList.toggle('mobile-nav-active')
      this.classList.toggle('bi-list')
      this.classList.toggle('bi-x')
    })
  
    /**
     * Scroll with ofset on links with a class name .scrollto
     */
    on('click', '.scrollto', function(e) {
      if (this.hash && select(this.hash)) {
        e.preventDefault()
        
        let body = select('body')
        if (body.classList.contains('mobile-nav-active')) {
          body.classList.remove('mobile-nav-active')
          let navbarToggle = select('.mobile-nav-toggle')
          navbarToggle.classList.toggle('bi-list')
          navbarToggle.classList.toggle('bi-x')
        }
        //scrollto(this.hash)
      }
    }, true)
  
    

      /**
   * Hero type effect
   */
  const typed = select('.typed')
  if (typed) {
    let typed_strings = typed.getAttribute('data-typed-items')
    typed_strings = typed_strings.split(',')
    new Typed('.typed', {
      strings: typed_strings,
      loop: true,
      typeSpeed: 100,
      backSpeed: 50,
      backDelay: 2000
    });
  }


    /**
     * Scroll with ofset on page load with hash links in the url
     */
    window.addEventListener('load', () => {
      if (window.location.hash) {
        if (select(window.location.hash)) {
          scrollto(window.location.hash)
        }
      }
    });
  
      
  
  })()