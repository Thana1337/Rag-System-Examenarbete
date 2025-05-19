// src/components/About.jsx
import React from 'react';
import { motion } from 'framer-motion';

const fadeInUp = {
  hidden: { opacity: 0, y: 20 },
  visible: { 
    opacity: 1, 
    y: 0, 
    transition: { duration: 0.6, ease: 'easeOut' } 
  }
};

const cardVariants = {
  hidden: { opacity: 0, scale: 0.8 },
  visible: { 
    opacity: 1, 
    scale: 1, 
    transition: { duration: 0.6, ease: 'easeOut' } 
  }
};

const timelineVariants = {
  hidden: { opacity: 0, x: -50 },
  visible: { 
    opacity: 1, 
    x: 0, 
    transition: { duration: 0.6, ease: 'easeOut' } 
  }
};

export default function About() {
  return (
    <section className="py-16 h-[100vh]">
      <div className="container mx-auto px-4 space-y-16">
        
        {/* Section Intro */}
        <motion.div
          className="text-center max-w-2xl mx-auto space-y-4"
          variants={fadeInUp}
          initial="hidden"
          whileInView="visible"
          viewport={{ once: true, amount: 0.5 }}
        >
          <h2 className="text-4xl font-bold text-white">
            About Our RAG System
          </h2>
          <p className="text-lg text-white">
            Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolores incidunt nihil id omnis ut dolor corporis sed accusantium recusandae odit.
          </p>
        </motion.div>

        {/* Services Cards */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          {[
            {
              title: 'Research',
              desc: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit.'
            },
            {
              title: 'Integrate',
              desc: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolores incidunt nihil id omnis'
            },
            {
              title: 'Deploy',
              desc: 'Lorem ipsum dolor sit amet, consectetur adipisicing elit. Dolores incidunt nihil id omnis ut dolor corporis sed accusantium recusandae odit.'
            }
          ].map((svc, i) => (
            <motion.div
              key={svc.title}
              className="bg-white rounded-2xl p-6 shadow-lg"
              variants={cardVariants}
              initial="hidden"
              whileInView="visible"
              viewport={{ once: true, amount: 0.3 }}
            >
              <h3 className="text-2xl font-semibold text-gray-800 mb-2">
                {svc.title}
              </h3>
              <p className="text-gray-600">{svc.desc}</p>
            </motion.div>
          ))}
        </div>
      </div>
    </section>
  );
}
