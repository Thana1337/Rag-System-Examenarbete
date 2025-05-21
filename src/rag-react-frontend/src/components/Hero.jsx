// src/components/Hero.jsx
import React, { useEffect } from 'react';
import { motion } from 'framer-motion';
import { Link } from 'react-router-dom';
import { damp } from 'three/src/math/MathUtils.js';

let hasPlayed = false;

export default function Hero() {
  const shouldAnimate = !hasPlayed;
  useEffect(() => {
    if (shouldAnimate) hasPlayed = true;
  }, [shouldAnimate]);

  const introDelay = 5;

  const headingTrans = {
    delay: introDelay - 1,
    duration: 1,
    type: 'spring',
  };

  const buttonTrans = {
    delay: introDelay - 1.7, 
    duration: 1,
    type: 'spring',
  };

  return (
    <section className="relative flex items-center justify-center h-screen overflow-hidden">
      <div
        className="container mx-auto px-4 flex flex-col items-center text-center space-y-4 z-30"
        style={{ fontFamily: "'Space Mono', monospace" }}
      >
        <motion.div
          className="transform origin-center"
          initial={shouldAnimate ? { x: '100vw', opacity: 0, scale: 0 } : undefined}
          animate={shouldAnimate ? { x: 0, opacity: 1, scale: 1 } : { x: 0, opacity: 1, scale: 1 }}
          transition={shouldAnimate ? headingTrans : { duration: 0 }}
        >
          <h1 className="text-5xl font-extrabold text-white">
            Retrieval-augmented
          </h1>
        </motion.div>

        <motion.div
          className="transform origin-center"
          initial={shouldAnimate ? { x: '-100vw', opacity: 0, scale: 0 } : undefined}
          animate={shouldAnimate ? { x: 0, opacity: 1, scale: 1 } : { x: 0, opacity: 1, scale: 1 }}
          transition={shouldAnimate ? headingTrans : { duration: 0 }}
        >
          <h1 className="text-5xl font-extrabold text-white">
            Generation <span className="text-white/70">(RAG)</span>
          </h1>
        </motion.div>

        {/* Buttons */}
        <motion.div
          className="flex space-x-4 mt-8"
          initial={shouldAnimate ? { y: '100vw', opacity: 0, scale: 0 } : undefined}
          animate={shouldAnimate ? { y: 0, opacity: 1, scale: 1 } : { y: 0, opacity: 1, scale: 1 }}
          transition={shouldAnimate ? buttonTrans : { duration: 0 }}
        >
          <Link
            to="/upload"
            className="
              px-6 py-3 bg-[#121212] text-white
              rounded-md font-medium
              hover:bg-[#484848] transition cursor-none
            "
          >
            Upload
          </Link>
          <Link
            to="/chat"
            className="
              px-6 py-3 bg-[#121212] text-white
              rounded-md font-medium
              hover:bg-[#484848] transition cursor-none
            "
          >
            Ask me
          </Link>
        </motion.div>
      </div>
    </section>
  );
}
