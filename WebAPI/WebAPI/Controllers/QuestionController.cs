﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : Controller
    {
        
        private readonly QuizDbContext _context;

        public QuestionController(QuizDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestions()
        {
            var random5Qns=await(_context.Questions
                .Select(x=> new
                {
                   QnId=x.QnId,
                   QnInWords=x.Qn,
                   ImageName=x.ImageName,
                   Options= new string[] {x.Option1,x.Option2,x.Option3,x.Option4}

                }).OrderBy(x=>Guid.NewGuid())
                .Take(6)
                ).ToListAsync();
            return Ok(random5Qns);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            return question;
        }
        [HttpPost]
        [Route("GetAnswers")]
        public async Task<ActionResult<Question>> RetrieveAnswers(int[] qnIds)
        {
            var answers = await (_context.Questions
                 .Where(x => qnIds.Contains(x.QnId))
                 .Select(y => new
                 {
                     QnId = y.QnId,
                     QnInWords = y.Qn,
                     ImageName = y.ImageName,
                     Options = new string[] { y.Option1, y.Option2, y.Option3, y.Option4 },
                      Answer = y.Answer
                 })).ToListAsync();
            return Ok(answers);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestion(int id, Question question)
        {
            if (id != question.QnId)
            {
                return BadRequest();
            }

            _context.Entry(question).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult>DeleteQuestion(int id)
        {
            var question=await _context.Questions.FindAsync(id);
            if(question == null)
            {
                return NotFound();
            }
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.QnId == id);
        }

    }
}
