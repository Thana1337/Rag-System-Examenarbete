import React from 'react';
import { motion } from 'framer-motion';

export default function TypingIndicator() {
  return (
    <motion.div
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      transition={{ duration: 0.3 }}
      className="
        self-start flex space-x-1 px-4 py-4
        bg-black/30 rounded-4xl
      "
    >
      {[0, 0.2, 0.4].map((delay, i) => (
        <motion.span
          key={i}
          initial={{ y: 0 }}
          animate={{ y: [-2, 2, -2] }}
          transition={{ repeat: Infinity, duration: 0.6, delay }}
          className="block w-1 h-1 bg-white rounded-full"
        />
      ))}
    </motion.div>
  );
}
