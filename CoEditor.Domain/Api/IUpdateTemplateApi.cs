﻿using CoEditor.Domain.Model;

namespace CoEditor.Domain.Api;

public interface IUpdateTemplateApi
{
    Task<Template> UpdateTemplateAsync(Template tmpl);
}
