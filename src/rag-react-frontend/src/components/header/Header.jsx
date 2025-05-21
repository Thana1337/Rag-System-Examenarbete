// src/components/Header.jsx
import React, { useEffect } from 'react';
import { motion } from 'framer-motion';

let headerPlayed = false;

export default function Header() {
  const shouldAnimate = !headerPlayed;
  useEffect(() => {
    if (shouldAnimate) headerPlayed = true;
  }, [shouldAnimate]);

  // match your scrambleDuration of 5s
  const delay = shouldAnimate ? 4 : 0;

  return (
    <motion.header
      // only use initial/animate on first load
      initial={shouldAnimate ? { y: -50, opacity: 0 } : undefined}
      animate={shouldAnimate ? { y: 0, opacity: 1 } : undefined}
      transition={
        shouldAnimate
          ? { delay, duration: 1.2, ease: 'easeOut' }
          : {}
      }
      className="relative z-20 overflow-hidden w-full h-16"
    >
      <div className="container mx-auto flex items-center justify-between px-8 h-full">
        <div className="text-xl italic flex gap-1 text-white">
          <p style={{ fontFamily: "'Space Mono', monospace" }}>
            Examensarbete.
          </p>
          <p style={{ fontFamily: "'Old Standard TT', serif" }}>
            rag-framework.
          </p>
        </div>

        <button
          onClick={() => (window.location.href = '/')}
          className="text-4xl font-normal focus:outline-none text-white"
          style={{ fontFamily: "'Mr Dafoe', cursive" }}
          aria-label="Go to homepage"
        >
          TS
        </button>
      </div>
    </motion.header>
  );
}
