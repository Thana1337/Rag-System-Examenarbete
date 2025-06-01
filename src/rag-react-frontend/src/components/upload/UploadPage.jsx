import React, { useState, useCallback } from 'react';
import { motion } from 'framer-motion';
import axios from 'axios';
import BackButton from '../BackButton';

export default function UploadPage() {
  const [files, setFiles]           = useState([]);      
  const [uploading, setUploading]   = useState(false);
  const [message, setMessage]       = useState('');
  const [isDragging, setIsDragging] = useState(false);
  const UPLOAD_URL                  = import.meta.env.VITE_UPLOAD_URL;

  const handleFiles = useCallback(newFiles => {
    const pdfs = Array.from(newFiles).filter(
      f => f.type === 'application/pdf'
    );
    setFiles(prev => {
      const all = [...prev];
      pdfs.forEach(f => {
        if (!all.some(existing => existing.name === f.name && existing.size === f.size)) {
          all.push(f);
        }
      });
      return all;
    });
    setMessage('');
  }, []);

  const onInputChange = e => handleFiles(e.target.files);

  const onDrop = e => {
    e.preventDefault();
    setIsDragging(false);
    handleFiles(e.dataTransfer.files);
  };

  const onDragOver = e => {
    e.preventDefault();
    if (!isDragging) setIsDragging(true);
  };

  const onDragLeave = () => {
    setIsDragging(false);
  };

  const removeFile = index => {
    setFiles(prev => prev.filter((_, i) => i !== index));
  };

  const handleUpload = async () => {
    if (files.length === 0) return;
    setUploading(true);
    setMessage('');

    try {
      const form = new FormData();
      files.forEach(f => form.append('files', f));

      await axios.post(UPLOAD_URL, form, {
        headers: { 'Content-Type': 'multipart/form-data' }
      });
      setMessage('✅ Upload successful!');
      setFiles([]);
    } catch (err) {
      console.error(err);
      setMessage('❌ Upload failed.');
    } finally {
      setUploading(false);
    }
  };

  return (
    <section className="flex items-center justify-center h-[80vh] overflow-hidden">
      <div
        className="container mx-auto px-4 flex flex-col items-center text-center space-y-6 z-30"
        style={{ fontFamily: "'Space Mono', monospace" }}
      >
        <motion.h1
          className="text-4xl md:text-5xl font-extrabold text-white"
          initial={{ y: -50, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ duration: 0.8, ease: 'easeOut' }}
        >
          Upload your PDFs
        </motion.h1>

        <motion.p
          className="text-white/80 max-w-md"
          initial={{ y: 30, opacity: 0 }}
          animate={{ y: 0, opacity: 1 }}
          transition={{ delay: 0.2, duration: 0.6 }}
        >
          Drag &amp; drop PDF files here, or click the area to select.
        </motion.p>

        {/* Drop zone */}
        <motion.div
          className={`w-full max-w-lg border-2 rounded-lg p-8 cursor-pointer
            ${isDragging
              ? 'border-white bg-[#484848]'
              : 'border-white/50 bg-[#121212]'}
          `}
          initial={{ scale: 0.9, opacity: 0 }}
          animate={{ scale: 1, opacity: 1 }}
          transition={{ delay: 0.4, duration: 0.6 }}
          onDrop={onDrop}
          onDragOver={onDragOver}
          onDragLeave={onDragLeave}
          onClick={() => document.getElementById('file-input').click()}
        >
          <input
            id="file-input"
            type="file"
            accept="application/pdf"
            multiple
            className="hidden"
            onChange={onInputChange}
            disabled={uploading}
          />
          <p className="text-white/70">
            {files.length > 0
              ? `${files.length} file(s) selected`
              : 'Click or drop PDFs here'}
          </p>
          {files.length > 0 && (
            <ul className="mt-4 text-left text-white space-y-1 max-h-32 overflow-auto">
              {files.map((f, i) => (
                <li key={i} className="flex justify-between items-center">
                  <span className="text-sm truncate max-w-xs">{f.name}</span>
                  <button
                    onClick={e => { e.stopPropagation(); removeFile(i); }}
                    className="text-red-400 hover:text-red-600 ml-2"
                    aria-label={`Remove ${f.name}`}
                  >
                    ×
                  </button>
                </li>
              ))}
            </ul>
          )}
        </motion.div>

        {/* Upload button */}
        <motion.button
          onClick={handleUpload}
          disabled={files.length === 0 || uploading}
          className={`
            px-6 py-3 rounded-md font-medium text-white transition-colors
            ${uploading
              ? 'bg-gray-500 cursor-not-allowed'
              : files.length > 0
                ? 'bg-[#121212] hover:bg-[#484848]'
                : 'bg-[#121212] opacity-50 cursor-not-allowed'
            }
          `}
          initial={{ scale: 0.9, opacity: 0 }}
          animate={{ scale: 1, opacity: 1 }}
          transition={{ delay: 0.6, duration: 0.6 }}
        >
          {uploading ? 'Uploading…' : 'Upload PDFs'}
        </motion.button>

        {/* Status message */}
        {message && (
          <motion.div
            className="text-white mt-2"
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ duration: 0.4 }}
          >
            {message}
          </motion.div>
        )}
      </div>
    </section>
  );
}
