import {atom, derive, unpack, derivation, isAtom, isDerivable} from 'derivable';
import {expect} from 'chai';



describe('Learning Derivable', () => {
    it('React but skip first', (done) => {
        const name = atom("Gary");

        name.react(
            v => {
                expect(v)
                    .to
                    .equal('Harry');

                done();
            },
            {
                skipFirst: true
            }
        );

        name.set("Harry");
    });

    it('React only once', (done) => {
        const name = atom("Gary");

        name.react(
            v => {
                expect(v)
                    .to
                    .equal('Gary');

                done();
            },
            {
                once: true
            }
        );

        name.set("Harry");
    });

    it('React to parent from derive', (done) => {
        const name = atom("Gary");
        const toUpper = name.derive(v => v.toUpperCase());

        toUpper.react(
            v => {
                expect(v)
                    .to
                    .equal('HARRY');

                done();
            },
            {
                skipFirst: true
            }
        );

        name.set("harry");
    });

    it('Grabbing atoms from atoms (destructing)', () => {
        const big = atom({
            first: 'Gary',
            last: 'Adams',

            address: {
                street: 'Shoshone Lane'
            }
        });

        const [first, address] = big.derive(['first', 'address']);

        expect(first.get())
            .to
            .equal('Gary');
        expect(address.get().street)
            .to
            .equal('Shoshone Lane');
    });
});

describe('Monads', () => {
    it('Rule one: something into an amplified somthing', () => {
        const simple = [1, 2, 3];
        const aArray = atom(simple);

        aArray.set(simple.reverse());
        expect(aArray.get())
            .to
            .equal(simple.reverse());
    });

    it('Rule two: pull out something from amplified something', () => {
        const simple = [1, 2, 3];
        const aArray = atom(simple);

        expect(unpack(aArray))
            .to
            .equal(simple);
    });

    it('Flattening monad (expected but not implemented)', () => {
        const inner = atom(1);
        const outer = atom(inner);

        expect(isAtom(unpack(outer)))
            .to
            .be
            .true;
    });

    it('Rule three: Give fn A, M<R> a bind turns M<A> to M<R>', () => {
        const name = atom('Gary');

        const lowerName = name.derive(
            v => {v.toLowerCase()}
        );

        expect(isDerivable(lowerName))
            .to
            .be
            .true;
    });
});

describe('From examples', () => {
    it('Derivation from noting', () => {
        const what = derivation(
            () => {
                return 5;
            }
        );

        expect(isDerivable(what))
            .to
            .be
            .true;
        expect(what.get())
            .to
            .equal(5);
    });

    it('Derivation with embedded', () => {
        const name = atom("Gary");

        const what = derivation(
            () => {
                return name;
            }
        );

        name.set('Harry');
        expect(what.get().get())
            .to
            .equal('Harry');
    });
});