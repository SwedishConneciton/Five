/**
 * General notes about Gulp 3
 *
 * Running tasks in sequence is not supported with Gulp 3 but planned 
 * for the next version (http://fettblog.eu/gulp-4-parallel-and-series/).
 */

import gulp from 'gulp';
import mocha from 'gulp-mocha';
import runSequence from 'run-sequence';
import source from 'vinyl-source-stream';
import babelify from 'babelify';
import browserify from 'browserify';
import del from 'del';
import {log as logger} from 'gulp-util';

gulp.task('clean', () => {
    del(
        ['wwwroot/*', '!wwwroot/web.config']
    )
    .then(
        (paths) => {
            logger(
                `Deleted files and folders [${paths}]`
            );
        }
    );
});

gulp.task('entry', () => {
    return gulp
        .src(
            ['Scripts/index.html']
        )
        .pipe(
            gulp.dest('wwwroot')
        );
});

gulp.task('bundle', () => {
    var b = browserify()
        .require(
            'Scripts/Device.jsx',
            { entry: true }
        )
        .transform(
            babelify,
            { presets: ["es2015", "react"] }
        );

    return b
        .bundle()
        .pipe(
            source('bundle.js')
        )
        .pipe(
            gulp.dest('wwwroot/scripts')
        );
});


gulp.task('test', () => {
    return gulp
        .src(
            ['Playground/**/*.Test.js']
        )
        .pipe(
            mocha({
                reporter: 'spec'
            })
        )
        .on(
            'error',
            () => {
                console.log('error');
            }
        );
});


gulp.task('default', (cb) => {
    runSequence(
        'clean',
        'entry',
        'bundle'
    );
});