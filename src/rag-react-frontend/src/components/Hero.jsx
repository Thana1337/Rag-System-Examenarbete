// src/components/Hero.jsx
import React from 'react';
import { motion } from 'framer-motion';
import { Link } from 'react-router-dom';

export default function Hero() {
  return (
    <section className="flex items-center justify-center h-screen overflow-hidden">
      <div
        className="container mx-auto px-4 flex flex-col items-center text-center space-y-4"
        style={{ fontFamily: "'Space Mono', monospace" }}
      >
        <motion.div
          className="transform origin-center"
          initial={{ x: '100vw', opacity: 0, scale: 0 }}
          animate={{ x: 0, opacity: 1, scale: 1 }}
          transition={{
            duration: 2,
            scale: { type: 'spring', stiffness: 100, damping: 20, bounce: 0.5 },
          }}
        >
          <h1 className="text-5xl font-extrabold text-white">
            Retrieval-augmented
          </h1>
        </motion.div>

        <motion.div
          className="transform origin-center"
          initial={{ x: '-100vw', opacity: 0, scale: 0 }}
          animate={{ x: 0, opacity: 1, scale: 1 }}
          transition={{
            duration: 2,
            scale: { type: 'spring', stiffness: 100, damping: 20, bounce: 0.5 },
          }}
        >
          <h1 className="text-5xl font-extrabold text-white">
            Generation <span className="text-white/70">(RAG)</span>
          </h1>
        </motion.div>

        {/* Buttons */}
        <div className="flex space-x-4 mt-8">
          <button
            disabled
            className="
              px-6 py-3 bg-white/10 text-white 
              rounded-md font-medium
              cursor-none
            "
          >
            Rapport
          </button>

          <Link
            to="/chat"
            className="
              px-6 py-3 bg-black/30 text-white 
              rounded-md font-medium
              hover:bg-white/20 transition cursor-none
            "
          >
            Ask me
          </Link>
        </div>
      </div>
    </section>
  );
}
