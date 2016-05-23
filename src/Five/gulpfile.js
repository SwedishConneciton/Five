/**
 * Just a handover to Gulp tasks written in ES6 that monkey-patches 
 * the require hook, automatically compiling on the fly.  Watch out 
 * if there is a .babelrc the settings there will override what is 
 * defined through babel-register
 */
require('babel-register')({
    presets: [ "es2015" ]
});

require('./gulpfile.es6.js');