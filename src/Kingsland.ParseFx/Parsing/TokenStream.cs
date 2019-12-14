﻿using Kingsland.ParseFx.Lexing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kingsland.ParseFx.Parsing
{

    public sealed class TokenStream
    {

        #region Constructors

        public TokenStream(IEnumerable<Token> source)
        {
            this.Source = source?.ToList() ?? throw new ArgumentNullException(nameof(source));
            this.Position = 0;
        }

        #endregion

        #region Properties

        private List<Token> Source
        {
            get;
            set;
        }

        public int Position
        {
            get;
            set;
        }

        public bool Eof
        {
            get
            {
                return (this.Source.Count == 0) ||
                       (this.Position >= this.Source.Count);
            }
        }

        #endregion

        #region Peek Methods

        public Token Peek()
        {
            if ((this.Source.Count == 0) ||
               (this.Position >= this.Source.Count))
            {
                throw new UnexpectedEndOfStreamException();
            }
            return this.Source[this.Position];
        }

        public T Peek<T>() where T : Token
        {
            if ((this.Source.Count == 0) ||
               (this.Position >= this.Source.Count))
            {
                throw new UnexpectedEndOfStreamException();
            }
            return (this.Source[this.Position] as T);
        }

        #endregion

        #region TryPeek Methods

        public bool TryPeek<T>() where T : Token
        {

            if ((this.Source.Count == 0) ||
               (this.Position >= this.Source.Count))
            {
                return false;
            }
            return (this.Source[this.Position] is T);
        }

        public bool TryPeek<T>(out T result) where T : Token
        {
            if ((this.Source.Count == 0) ||
                (this.Position >= this.Source.Count))
            {
                throw new UnexpectedEndOfStreamException();
            }
            var peek = this.Source[this.Position] as T;
            if (peek == null)
            {
                result = null;
                return false;
            }
            result = peek;
            return true;
        }

        public bool TryPeek<T>(Func<T, bool> predicate, out T result) where T : Token
        {

            if ((this.Source.Count == 0) ||
                (this.Position >= this.Source.Count))
            {
                throw new UnexpectedEndOfStreamException();
            }
            var peek = this.Source[this.Position] as T;
            if ((peek != null) && predicate(peek))
            {
                result = peek;
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region Read Methods

        public Token Read()
        {
            var value = this.Peek();
            this.Position += 1;
            return value;
        }

        public T Read<T>() where T : Token
        {
            var token = this.Peek();
            if (token is T cast)
            {
                this.Read();
                return cast;
            }
            throw new UnexpectedTokenException(token);
        }

        #endregion

        #region TryRead Methods

        public bool TryRead<T>(out T result) where T : Token
        {
            if (this.TryPeek<T>(out result))
            {
                this.Read();
                return true;
            }
            return false;
        }

        #endregion

        #region Backtrack Methods

        /// <summary>
        /// Moves the stream position back a token.
        /// </summary>
        public void Backtrack()
        {
            this.Backtrack(1);
        }

        /// <summary>
        /// Moves the stream position back the specified number of tokens.
        /// </summary>
        public void Backtrack(int count)
        {
            if (this.Position < count)
            {
                throw new InvalidOperationException();
            }
            this.Position -= count;
        }

        #endregion

        #region Object Interface

        public override string ToString()
        {
            var result = new StringBuilder();
            var count = 0;
            for (var i = Math.Max(0, this.Position - 5); i < Math.Min(this.Source.Count, this.Position + 5); i++)
            {
                if (count > 0)
                {
                    result.Append(" ");
                    count += 1;
                }
                if (i == this.Position)
                {
                    result.Append(">>>");
                    count += 3;
                }
                result.Append(this.Source[i]);
            }
            return string.Format("Current = '{0}'", result.ToString());
        }

        #endregion

    }

}
