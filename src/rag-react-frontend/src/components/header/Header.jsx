// src/components/Header.jsx
import React from 'react';
import { motion } from 'framer-motion';
import BackButton from '../BackButton';

const headerVariants = {
  initial: {
    height: '120vh',
    backgroundColor: '#000000',
  },
  animate: {
    height: '4rem',
    backgroundColor: '#1A1A1A',
    transition: {
      duration: 2.5,
      ease: 'easeInOut',
      when: 'beforeChildren',
      staggerChildren: 0.1,
    },
  },
};

const childVariants = {
  initial: { opacity: 0, y: -10 },
  animate: { opacity: 1, y: 0, transition: { duration: 0.4, ease: 'easeOut' } },
};

export default function Header() {
  return (
    <motion.header
      variants={headerVariants}
      initial="initial"
      animate="animate"
      className="overflow-hidden w-full"
    >
      <div className="container mx-auto flex items-center justify-between px-8 h-full">
        <motion.div variants={childVariants} className="text-xl italic flex gap-1 text-white">
          <p style={{ fontFamily: "'Space Mono', monospace" }}>
            Examensarbete.
          </p>
          <p style={{ fontFamily: "'Old Standard TT', serif" }}>
            rag-framework.
          </p>
        </motion.div>

        <motion.button
          variants={childVariants}
          onClick={() => (window.location.href = '/')}
          className="text-4xl font-normal focus:outline-none text-white"
          style={{ fontFamily: "'Mr Dafoe', cursive",  }}
          aria-label="Go to homepage"
        >
          TS
        </motion.button>
      </div>
    </motion.header>
  );
}
